using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WordsWithinWords
{
    public class AnalyserWordWithinWord : Analyser, IAnalyser
    {
        public AnalyserWordWithinWord(Dictionaries dictionaries, Language language) : base(dictionaries, AnalysisType.WordsWithinWords, language)
        {
        }

        private List<WordWithinWord> _wordWithinWords = new List<WordWithinWord>();

        public void Start()
        {
            foreach (var word in WordSet)
            {
                var ww = new WordWithinWord(word, WordSet);
                _wordWithinWords.Add(ww);
            }

            var boats = _wordWithinWords.Where(e => e.Word == "boats").ToList();

            Console.WriteLine($"Writing output {OutputPath}");

            File.WriteAllText(OutputPath, "");
            _wordWithinWords = _wordWithinWords.OrderByDescending(e => e.WordsWithinWord.Count).ToList();

            var totalWordsWith = _wordWithinWords.Count(e => e.WordsWithinWord.Count > 0);
            var totalWordsWithout = _wordWithinWords.Count(e => e.WordsWithinWord.Count == 0);
            var totalWord = WordSet.Count;

            var percentageWith = ((float)totalWordsWith / totalWord) * 100;
            var percentageWithout = ((float)totalWordsWithout / totalWord) * 100;

            var total = percentageWith + percentageWithout;

            var outstr0 = $"{totalWord} total words in dictionary";
            var outstr1 = $"{totalWordsWith} total words with words {percentageWith} %";
            var outstr2 = $"{totalWordsWithout} total words without words {percentageWithout} %";


            Console.WriteLine(outstr0);
            Console.WriteLine(outstr1);
            Console.WriteLine(outstr2);

            var outstrs = new List<string>();
            outstrs.Add(outstr0);
            outstrs.Add(outstr1);
            outstrs.Add(outstr2);

            File.AppendAllLines(OutputPath, outstrs,Encoding.UTF8);            


            foreach (var word in _wordWithinWords)
            {
                if (word.HasAll)
                {
                    File.AppendAllText(OutputPath, word.Output, Encoding.UTF8);
                }
            }

            Console.WriteLine($"Done writing output {OutputPath}");
        }
    }
}