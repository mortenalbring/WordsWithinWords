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
                if (word.Length <= 4)
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

            _wordWithinWords = _wordWithinWords.OrderByDescending(e => e.Depth).Take(10).ToList();

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


            // WordNodesAndEdges.Build(this._language, _wordWithinWords, Dictionaries, "WordNodesRecursiveJson");

            var wdepthList = wdict.Values.Where(e => e.Depth > 0).Select(e => e).OrderByDescending(e => e.Depth).ToList();
            AppendOutput($"{wdepthList.Count:N0} word chains found");

            wdepthList = wdepthList.Where(e => e.Depth > 2).ToList();

            foreach (var word in wdepthList)
            {
                var wordChain = word.GetWordChain();

                var wordChainOutput = wordChain.Count + "\t" + word.Word + "\t" + string.Join(",", wordChain) + "\n";

                //File.AppendAllText(OutputPath, word.Word + "\t" + string.Join(",", wordChain) + "\n", Encoding.UTF8);


                //AppendOutput(wordChainOutput);


                // Console.WriteLine(word.Word + "\t" + string.Join(",", wordChain));
            }


            Console.WriteLine("Done");
        }
    }
}