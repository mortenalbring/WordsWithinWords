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


            FindClusteredWords();


            Console.WriteLine($"Writing output {OutputPath}");

            _wordWithinWords = _wordWithinWords.OrderByDescending(e => e.Depth).Take(10).ToList();

            foreach (var word in _wordWithinWords)
            {
                var str = word.Depth + "\t" + word.Word + "\t";
                var wordChain = word.GetWordChain();

                foreach (var w in wordChain)
                {
                    str = str + w + "\t";
                }

                str += "\n";

                File.AppendAllText(OutputPath, str, Encoding.UTF8);
            }


            Console.WriteLine($"Done writing output {OutputPath}");
        }

        private void FindClusteredWords()
        {
            Console.WriteLine("Finding clusters");
            var clusterInfo = new Dictionary<string, List<string>>();
            var biggestClusterWord = "";
            var biggestCluster = new List<string>();
            var alreadyNoted = new HashSet<string>();

            foreach (var word in _wordWithinWords)
            {
                if (alreadyNoted.Contains(word.Word))
                {
                    continue;
                }

                var dictKey = word.Word;
                var cluster = word.GetClusters();
                clusterInfo.Add(dictKey, cluster);
                foreach (var c in cluster)
                {
                    alreadyNoted.Add(c);
                }

                if (cluster.Count > biggestCluster.Count)
                {
                    biggestCluster = cluster;
                    biggestClusterWord = dictKey;
                }
            }

            var interestingClusters = new List<WordWithinWord>();
            var topWord = _wordWithinWords.FirstOrDefault(e => e.Word == biggestClusterWord);
            interestingClusters.Add(topWord);
            foreach (var bcw in biggestCluster)
            {
                var w = _wordWithinWords.FirstOrDefault(e => e.Word == bcw);
                interestingClusters.Add(w);
            }

            WordNodesAndEdges.Build(_language, interestingClusters, Dictionaries, "ClusterJson");

            Console.WriteLine(clusterInfo.Count + " clusters found");
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


          //  WordNodesAndEdges.Build(this._language, _wordWithinWords, Dictionaries, "WordNodesRecursiveJson");

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