using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace WordsWithinWords.Analysers
{
    public class AnalyserRecursive : Analyser, IAnalyser
    {
        public AnalyserRecursive(Dictionaries dictionaries, Language language) : base(dictionaries, AnalysisType.WordsWithinWordsRecursive, language)
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

            Sw.Stop();
            Sw.Restart();

            DoRecursiveWords();


            //FindClusteredWords();

            var deepest = _wordWithinWords.Select(e => e.Depth).Max();
            var deepestWords = _wordWithinWords.Where(e => e.Depth == deepest).ToList();


            Console.WriteLine($"Writing output {OutputPath}");

           // _wordWithinWords = _wordWithinWords.OrderByDescending(e => e.Depth).Take(10).ToList();

            var deepWordChain = new List<WordWithinWord>();
            var g = 1;
            foreach (var word in _wordWithinWords)
            {
                var str = word.Depth + "\t" + word.Word + "\t";
                var wordChain = word.GetWordChain();
                var wordHashShet = new HashSet<string>();
                foreach (var wc in wordChain)
                {
                    wordHashShet.Add(wc);
                }

                string parentWord = word.Word;
                foreach (var wc in wordChain)
                {
                    var ww = new WordWithinWord(wc,wordHashShet);
                    ww.Group = g;
                    deepWordChain.Add(ww);
                }
                
                word.Group = g;
                deepWordChain.Add(word);

                g++;
                // File.AppendAllText(OutputPath, str, Encoding.UTF8);
            }

            WordNodesAndEdges.Build(Language, deepWordChain, Dictionaries, "deepwordchain");

            Console.WriteLine($"Done writing output {OutputPath}");
        }

        private void FindClusteredWords()
        {
            Console.WriteLine("Finding clusters");


            var sortedWords = _wordWithinWords.Where(e => e.WordsWithinWordsRecursive.Count > 0).OrderByDescending(e => e.Word.Length).ToList();

            //var clusterLists = new List<List<string>>();
            var i = 0;

            var clusterLists = new HashSet<HashSet<string>>();

            foreach (var word in sortedWords)
            {
                var wordList = word.GetWordList(1);
                var matchingCluster = new HashSet<string>();

                var newCluster = true;

                foreach (var w in wordList)
                {
                    foreach (var c in clusterLists)
                    {
                        if (c.Contains(w))
                        {
                            matchingCluster = c;
                            newCluster = false;
                            break;
                        }
                    }

                    if (!newCluster)
                    {
                        break;
                    }
                }

                if (!newCluster)
                {
                    foreach (var w in wordList.Where(w => !matchingCluster.Contains(w)))
                    {
                        matchingCluster.Add(w);
                    }
                }

                if (newCluster)
                {
                    matchingCluster = wordList;
                    clusterLists.Add(matchingCluster);
                }

                i++;

                var longestCluster = clusterLists.Max(e => e.Count);

                Console.WriteLine($"{i} / {sortedWords.Count} {longestCluster} longest cluster found, {clusterLists.Count} total clusters");
            }

            var clusterNum = clusterLists.Count;


            var topClusters = clusterLists.OrderByDescending(e => e.Count).ToList();
            var interestingClusters = new List<WordWithinWord>();

            var g = 0;
            foreach (var c in topClusters)
            {
                foreach (var w in c)
                {
                    var ww = _wordWithinWords.FirstOrDefault(e => e.Word == w);
                    ww.Group = g;
                    interestingClusters.Add(ww);
                }

                g++;
            }


            WordNodesAndEdges.Build(_language, interestingClusters, Dictionaries, "ClusterJson");
        }


        private void DoRecursiveWords()
        {
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


            var hasAll = _wordWithinWords.Where(e => e.HasAll).OrderByDescending(e => e.Word.Length).ToList();
            var doubleHasAll = new List<WordWithinWord>();

            Dictionary<WordWithinWord,int> GrandChildCount = new Dictionary<WordWithinWord,int>();
            var mostGrandChildren = 0;
            WordWithinWord MostgrandWords;
            
            foreach (var w in _wordWithinWords)
            {
                var grandChildCount = 0;
                
                if (w.WordsWithinWordsRecursive.Count == 0)
                {
                    continue;
                }
                foreach (var ww in w.WordsWithinWordsRecursive)
                {
                    grandChildCount = ww.WordsWithinWordsRecursive.Count;
                }

                GrandChildCount.Add(w,grandChildCount);
            }

            var mostGrandChildrenKey = GrandChildCount.OrderByDescending(e => e.Value).Select(e => e.Value).First();
            var mostGrandChildrenVals = GrandChildCount.Where(e => e.Value == mostGrandChildrenKey).Select(e => e.Key).ToList(); 

            var xxx = 42;

            // WordNodesAndEdges.Build(this._language, _wordWithinWords, Dictionaries, "WordNodesRecursiveJson");

            var wdepthList = wdict.Values.Where(e => e.Depth > 0).Select(e => e).OrderByDescending(e => e.Depth).ToList();
            
            
            
            AppendOutput($"{wdepthList.Count:N0} word chains found");

          //  wdepthList = wdepthList.Where(e => e.Depth == 8).ToList();

            var wdepthlev9 = wdepthList.Where(e => e.Depth == 9).ToList();
            var wdepthlev8 = wdepthList.Where(e => e.Depth == 8).ToList();
            
            var chainWords9 = new List<WordWithinWord>();
            var chainWords8 = new List<WordWithinWord>();

            var g = 0;
            foreach (var word in wdepthlev9)
            {
                var wordChain = word.GetWordChain();
                foreach (var c in wordChain)
                {
                    var w = _wordWithinWords.FirstOrDefault(e => e.Word == c);
                    w.Group = g;
                    chainWords9.Add(w);
                }

                g++;
                
                var wordChainOutput = wordChain.Count + "\t" + word.Word + "\t" + string.Join(",", wordChain) + "\n";
                AppendOutput(wordChainOutput);
            }
            WordNodesAndEdges.Build(this._language, chainWords9, Dictionaries, "WordChainEnglishSowpods20210914Depth9");
            
            foreach (var word in wdepthlev8)
            {
                var wordChain = word.GetWordChain();
                foreach (var c in wordChain)
                {
                    if (chainWords9.Select(e => e.Word).ToList().Contains(c))
                    {
                        continue;
                    }
                    var w = _wordWithinWords.FirstOrDefault(e => e.Word == c);
                    w.Group = g;
                    chainWords8.Add(w);
                }

                g++;
                
                var wordChainOutput = wordChain.Count + "\t" + word.Word + "\t" + string.Join(",", wordChain) + "\n";
                AppendOutput(wordChainOutput);
            }
            WordNodesAndEdges.Build(this._language, chainWords8, Dictionaries, "WordChainEnglishSowpods20210914Depth8");

            


            Console.WriteLine("Done");
        }
    }
}