using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WordsWithinWords.Analysers
{
    public class AnalyserAlphabetical : Analyser, IAnalyser
    {
        public AnalyserAlphabetical(Dictionaries dictionaries, Language language) : base(dictionaries, AnalysisType.Alphabetical, language)
        {
        }

        public void Start()
        {
            var bestWords = new List<string>();
            var longestWordCount = 0;
            var mostDistinctChars = 0;

            foreach (var word in WordSet)
            {
                var chars = word.ToList().OrderBy(e => e).ToList();
                var alphaword = "";
                foreach (var c in chars)
                {
                    alphaword = alphaword + c;
                }

                if (alphaword == word)
                {
                    bestWords.Add(word);
                }

                if (word.Length > longestWordCount)
                {
                    longestWordCount = word.Length;
                }

                var distinctChars = word.Distinct().Count();
                var noRepeats = word.Distinct().Count() == word.Length;

                if (distinctChars > mostDistinctChars && noRepeats)
                {
                    mostDistinctChars = distinctChars;
                }

            }

            var longestWords = WordSet.Where(e => e.Length == longestWordCount).ToList();
            var mostDistinctCharWords = WordSet.Where(e => e.Distinct().Count() == mostDistinctChars && e.Length == mostDistinctChars).ToList();
            Console.WriteLine();


            File.WriteAllText(OutputPath, "");
            var outstr0 = $"{WordSet.Count:N0} total words in dictionary";
            var outstr4 = $"There are {longestWords.Count:N0} words with the most number of letters ({longestWordCount})";
            var outstr1 = $"There are {bestWords.Count:N0} with all letters in alphabetical order";

            var outstr2 = $"There are {mostDistinctCharWords.Count:N0} words with the most number of distinct characters ({mostDistinctChars}) with no repeats";


            var outstrs = new List<string>();
            outstrs.Add(outstr0);
            outstrs.Add(outstr2);
            outstrs.AddRange(mostDistinctCharWords);
            outstrs.Add(outstr4);
            outstrs.AddRange(longestWords);
            outstrs.Add(outstr1);

            foreach (var str in outstrs)
            {
                Console.WriteLine(str);
                File.AppendAllText(OutputPath, str + "\n", Encoding.UTF8);
            }



            var top10 = bestWords.OrderByDescending(e => e.Length).ToList();

            foreach (var t in top10)
            {
                Console.WriteLine(t);
                File.AppendAllText(OutputPath, t.Length + "\t" + t + "\n",Encoding.UTF8);
            }
        }
    }
}