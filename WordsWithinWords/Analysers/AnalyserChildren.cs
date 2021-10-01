﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace WordsWithinWords.Analysers
{
    public class AnalyserChildren : Analyser, IAnalyser
    {
        public AnalyserChildren(Dictionaries dictionaries, Language language) : base(dictionaries, AnalysisType.WordsWithinWordsRecursive, language)
        {
            this._language = language;
        }

        private readonly Language _language;
        private List<WordWithinWord> _wordWithinWords = new List<WordWithinWord>();

        public void Start()
        {
            Sw.Start();
            File.WriteAllText(OutputPath, "");

            var index = 0;
            foreach (var word in WordSet)
            {
                index++;
                if (word.Length <= 1)
                {
                    continue;
                }

                var wwr = new WordWithinWord(word, WordSet);
                _wordWithinWords.Add(wwr);
                Progress.OutputTimeRemaining(index, WordSet.Count, Sw);
            }
            
            var wdict = new Dictionary<string, WordWithinWord>();
            foreach (var w in _wordWithinWords)
            {
                if (w.HasAny)
                {
                    wdict.Add(w.Word, w);
                }
            }

            var i = 0;

            foreach (var w in wdict)
            {
                foreach (var s in w.Value.WordsWithinWord)
                {
                    var exists = wdict.ContainsKey(s);
                    if (exists)
                    {
                        var elem = wdict[s];
                        w.Value.WordsWithinWordsRecursive.Add(elem);
                    }
                }

                i++;

                Progress.OutputTimeRemaining(i, wdict.Count, Sw);
            }


            Sw.Stop();
            Sw.Restart();

            FindParents();
          //  FindChildren();


            Console.WriteLine($"Done writing output {OutputPath}");
        }


        private void FindParents()
        {
            var output = new List<WordWithinWordParent>();

            var output2 = new Dictionary<string, List<string>>();
            
            foreach (var w in _wordWithinWords)
            {
                foreach (var www in w.WordsWithinWordsRecursive)
                {
                    
                    var elem = 
                    
                    var ww = new WordWithinWordParent(www);
                    var exists = output.FirstOrDefault(e => e.Word == www.Word);
                    if (exists != null)
                    {
                        var pexists = exists.Parents.FirstOrDefault(e => e.Word == w.Word);
                        if (pexists != null)
                        {
                            exists.Parents.Add(w);    
                        }
                    }
                    else
                    {
                        ww.Parents = new List<WordWithinWord>();
                        ww.Parents.Add(w);
                        output.Add(ww);
                    }

                }

            }
            
            var mostParents = 0;
            WordWithinWordParent mostParentsWord;
            
            foreach (var o in output)
            {
                if (o.Parents.Count > mostParents)
                {
                    mostParents = o.Parents.Count;
                    mostParentsWord = o;
                }
            }
        }
        
        private void FindChildren()
        {
           

            
            for (int j = 1; j < 20; j++)
            {
                var mostNChildren1 = GetMostNChildren(j);
                var infoDict = new Dictionary<int, int>();
                var mostNChildren1List = GetSingleNChildrenList(mostNChildren1, j);
                
                WordNodesAndEdges.Build(this._language, mostNChildren1List, Dictionaries, "mostNChildren" + j);
                
                Console.WriteLine(j + "\t" + mostNChildren1List.Count);
            }
            
            
            var wordsWithMostChildren = GetMostChildren();
            var wordsWithMostGrandchildren = GetMostGrandchildren();
            var wordsWithMostGreatGrandchildren = GetMostGreatGrandchildren();

            WordNodesAndEdges.Build(this._language, wordsWithMostChildren, Dictionaries, "MostChildren");
            WordNodesAndEdges.Build(this._language, wordsWithMostGrandchildren, Dictionaries, "MostGrandChildren");
            WordNodesAndEdges.Build(this._language, wordsWithMostGreatGrandchildren, Dictionaries, "MostGreatGrandChildren");
        }

        private List<WordWithinWord> GetMostChildren()
        {
            Dictionary<WordWithinWord, int> childCount = new Dictionary<WordWithinWord, int>();
            var mostGrandChildren = 0;
            WordWithinWord MostgrandWords;

            foreach (var w in _wordWithinWords)
            {
                if (w.WordsWithinWordsRecursive.Count == 0)
                {
                    continue;
                }


                childCount.Add(w, w.WordsWithinWordsRecursive.Count);
            }

            var mostChildrenCount = childCount.OrderByDescending(e => e.Value).Select(e => e.Value).First();
            var mostChildrenVals = childCount.Where(e => e.Value == mostChildrenCount).Select(e => e.Key).ToList();

            var builtOutput = new List<WordWithinWord>();
            foreach (var w in mostChildrenVals)
            {
                w.Group = 1;
                builtOutput.Add(w);
                foreach (var ww in w.WordsWithinWordsRecursive)
                {
                    ww.Group = 2;
                    builtOutput.Add(ww);
                }
            }

            return builtOutput;
        }

        private List<WordWithinWord> GetSingleNChildrenList(Dictionary<WordWithinWord, List<WordWithinWord>> wordDict, int maxDepth)
        {
            var mostList = new List<WordWithinWord>();
            var mostUniqueCount = 0;

            foreach (var d in wordDict)
            {
                var unq = d.Value.Select(e => e.Word).Distinct().ToList().Count;
                if (unq > mostUniqueCount)
                {
                    mostUniqueCount = unq;
                }
            }

            foreach (var d in wordDict)
            {
                var unq = d.Value.Select(e => e.Word).Distinct().ToList().Count;
                if (unq == mostUniqueCount)
                {
                    mostList.Add(d.Key);
                }
            }

            var outputList = new List<WordWithinWord>();
            foreach (var m in mostList)
            {
                m.Group = 1;
                outputList.Add(m);
                var subwords = DoGetList2(m, 0, maxDepth);
                outputList.AddRange(subwords);
            }

            //var mostList = wordDict.Where(e => e.Value.Count == mostDescedents).Select(e => e.Key).ToList();

            return outputList;
        }

        private Dictionary<WordWithinWord, List<WordWithinWord>> GetMostNChildren(int maxDepth)
        {
            var output = new List<WordWithinWord>();
            Dictionary<WordWithinWord, List<WordWithinWord>> childCount = new Dictionary<WordWithinWord, List<WordWithinWord>>();


            foreach (var w in _wordWithinWords)
            {
                var descendantWords = DoGetList(w, 0, maxDepth);

                childCount.Add(w, descendantWords);
            }


            return childCount;
        }

        private List<WordWithinWord> DoGetList2(WordWithinWord word, int depth, int maxDepth)
        {
            var output = new List<WordWithinWord>();
            word.Group = depth + 1;
            if (depth < maxDepth)
            {
                foreach (var w in word.WordsWithinWordsRecursive)
                {
                    w.Group = depth + 2;
                    output.Add(w);

                    var subout = DoGetList2(w, depth + 1, maxDepth);
                    foreach (var s in subout)
                    {
                        output.Add(s);
                    }
                }
            }

            return output;
        }

        private List<WordWithinWord> DoGetList(WordWithinWord word, int depth, int maxDepth)
        {
            word.Group = depth;

            var output = word.WordsWithinWordsRecursive.ToList();

            if (depth < maxDepth)
            {
                foreach (var w in word.WordsWithinWordsRecursive)
                {
                    w.Group = depth;
                    var suboutput = DoGetList(w, depth + 1, maxDepth);
                    output.AddRange(suboutput);
                }
            }

            return output;
        }

        private List<WordWithinWord> GetMostGreatGrandchildren()
        {
            Dictionary<WordWithinWord, int> GrandChildCount = new Dictionary<WordWithinWord, int>();
            var mostGrandChildren = 0;
            WordWithinWord MostgrandWords;

            foreach (var w in _wordWithinWords)
            {
                var grandChildCount = 0;

                var grandChildrenNames = new List<string>();


                if (w.WordsWithinWordsRecursive.Count == 0)
                {
                    continue;
                }

                foreach (var ww in w.WordsWithinWordsRecursive)
                {
                    foreach (var www in ww.WordsWithinWordsRecursive)
                    {
                        foreach (var wwww in www.WordsWithinWordsRecursive)
                        {
                            if (!grandChildrenNames.Contains(wwww.Word))
                            {
                                grandChildrenNames.Add(wwww.Word);
                            }
                        }
                    }
                    //grandChildCount = grandChildCount + ww.WordsWithinWordsRecursive.Count;
                }

                grandChildCount = grandChildrenNames.Count;
                GrandChildCount.Add(w, grandChildCount);
            }

            var mostGrandChildrenKey = GrandChildCount.OrderByDescending(e => e.Value).Select(e => e.Value).First();

            for (int i = 0; i <= mostGrandChildrenKey; i++)
            {
                var countList = GrandChildCount.Where(e => e.Value == i).ToList();
                Console.WriteLine(i + "\t" + countList.Count);
            }

            var xx = 42;


            var mostGrandChildrenVals = GrandChildCount.Where(e => e.Value == mostGrandChildrenKey).Select(e => e.Key).ToList();

            var builtOutput = new List<WordWithinWord>();
            var g = 1;
            var gg = 1;
            foreach (var w in mostGrandChildrenVals)
            {
                w.Group = 1;
                builtOutput.Add(w);
                foreach (var ww in w.WordsWithinWordsRecursive)
                {
                    ww.Group = 2;
                    builtOutput.Add(ww);

                    foreach (var www in ww.WordsWithinWordsRecursive)
                    {
                        www.Group = 3;
                        builtOutput.Add(www);
                    }
                }
            }

            var xxx = 42;
            return builtOutput;
        }

        private List<WordWithinWord> GetMostGrandchildren()
        {
            Dictionary<WordWithinWord, int> GrandChildCount = new Dictionary<WordWithinWord, int>();
            var mostGrandChildren = 0;
            WordWithinWord MostgrandWords;

            foreach (var w in _wordWithinWords)
            {
                var grandChildCount = 0;

                var grandChildrenNames = new List<string>();


                if (w.WordsWithinWordsRecursive.Count == 0)
                {
                    continue;
                }

                foreach (var ww in w.WordsWithinWordsRecursive)
                {
                    foreach (var www in ww.WordsWithinWordsRecursive)
                    {
                        if (!grandChildrenNames.Contains(www.Word))
                        {
                            grandChildrenNames.Add(www.Word);
                        }
                    }
                    //grandChildCount = grandChildCount + ww.WordsWithinWordsRecursive.Count;
                }

                grandChildCount = grandChildrenNames.Count;
                GrandChildCount.Add(w, grandChildCount);
            }


            var mostGrandChildrenKey = GrandChildCount.OrderByDescending(e => e.Value).Select(e => e.Value).First();

            for (int i = 0; i <= mostGrandChildrenKey; i++)
            {
                var countList = GrandChildCount.Where(e => e.Value == i).ToList();
                Console.WriteLine(i + "\t" + countList.Count);
            }

            var xx = 42;


            var mostGrandChildrenVals = GrandChildCount.Where(e => e.Value == mostGrandChildrenKey).Select(e => e.Key).ToList();

            var builtOutput = new List<WordWithinWord>();
            var g = 1;
            var gg = 1;
            foreach (var w in mostGrandChildrenVals)
            {
                w.Group = g;
                g++;
                builtOutput.Add(w);
                gg = g + 1;
                foreach (var ww in w.WordsWithinWordsRecursive)
                {
                    ww.Group = gg;
                    gg++;
                    builtOutput.Add(ww);
                }
            }

            var xxx = 42;
            return builtOutput;
        }
    }
}