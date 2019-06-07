using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace WordsWithinWords
{
    public class AnalyserRecursive : Analyser, IAnalyser
    {
        public AnalyserRecursive(WordLists wordLists, Language language) : base(wordLists, AnalysisType.WordsWithinWordsRecursive, language)
        {

        }

        public new void Start()
        {
            var sw = new Stopwatch();
            sw.Start();


            var index = 0;
            foreach (var word in WordSet)
            {
                index++;
                if (word.Length <= 2)
                {
                    continue;
                }

                var wwr = new WordWithinWord(word, WordSet);
                this.WordWithinWords.Add(wwr);
                Progress.OutputTimeRemaining(index, WordSet.Count, sw);
            }


            sw.Stop();
            sw.Restart();

            DoRecursiveWords(sw);


            Console.WriteLine($"Writing output {OutputPath}");

            File.WriteAllText(OutputPath, "");
            this.WordWithinWords = this.WordWithinWords.OrderByDescending(e => e.WordsWithinWord.Count).ToList();

            foreach (var word in this.WordWithinWords)
            {
                if (word.HasAll)
                {
                    File.AppendAllText(OutputPath, word.Output, Encoding.UTF8);
                }

            }

            Console.WriteLine($"Done writing output {OutputPath}");
        }

        private void DoRecursiveWords(Stopwatch sw)
        {



            var wdict = new Dictionary<string, WordWithinWord>();
            foreach (var w in this.WordWithinWords)
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

                Progress.OutputTimeRemaining(i, wdict.Count, sw);
            }


            WordNodesAndEdges.Build(this.WordWithinWords, this.WordLists);

            var wdepthList = wdict.Values.Where(e => e.Depth > 0).Select(e => e).OrderByDescending(e => e.Depth).ToList();

            foreach (var word in wdepthList)
            {
                var wordChain = word.GetWordChain();

                Console.WriteLine(word.Word + "\t" + string.Join(",", wordChain));
            }



            Console.WriteLine("Done");
        }



    }
}
