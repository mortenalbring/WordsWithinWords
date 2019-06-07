using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WordsWithinWords
{
    public class AnalyserRecursive : Analyser, IAnalyser
    {
        public AnalyserRecursive(Dictionaries dictionaries, Language language) : base(dictionaries, AnalysisType.WordsWithinWordsRecursive, language)
        {
        }

        private List<WordWithinWord> WordWithinWords = new List<WordWithinWord>();

        public void Start()
        {
            Sw.Start();


            var index = 0;
            foreach (var word in WordSet)
            {
                index++;
                if (word.Length <= 2)
                {
                    continue;
                }

                var wwr = new WordWithinWord(word, WordSet);
                WordWithinWords.Add(wwr);
                Progress.OutputTimeRemaining(index, WordSet.Count, Sw);
            }


            Sw.Stop();
            Sw.Restart();

            DoRecursiveWords();


            Console.WriteLine($"Writing output {OutputPath}");

            File.WriteAllText(OutputPath, "");
            WordWithinWords = WordWithinWords.OrderByDescending(e => e.WordsWithinWord.Count).ToList();

            foreach (var word in WordWithinWords)
            {
                if (word.HasAll)
                {
                    File.AppendAllText(OutputPath, word.Output, Encoding.UTF8);
                }
            }

            Console.WriteLine($"Done writing output {OutputPath}");
        }

        private void DoRecursiveWords()
        {
            var wdict = new Dictionary<string, WordWithinWord>();
            foreach (var w in WordWithinWords)
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


            WordNodesAndEdges.Build(WordWithinWords, Dictionaries);

            var wdepthList = wdict.Values.Where(e => e.Depth > 0).Select(e => e).OrderByDescending(e => e.Depth).ToList();

            foreach (var word in wdepthList)
            {
                var wordChain = word.GetWordChain();

                //todo write to file

                Console.WriteLine(word.Word + "\t" + string.Join(",", wordChain));
            }


            Console.WriteLine("Done");
        }
    }
}