using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WordsWithinWords
{
    public class AnalyserAlphabetical : Analyser, IAnalyser
    {
        public AnalyserAlphabetical(Dictionaries dictionaries, Language language) : base(dictionaries, AnalysisType.Alphabetical, language)
        {
        }

        public void Start()
        {
            var bestWords = new List<string>();

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
            }

            Console.WriteLine($"There are {bestWords.Count} with all letters in alphabetical order");

            File.WriteAllText(OutputPath, "");
            var outstr0 = $"{bestWords} total words in dictionary";
            var outstrs = new List<string>();
            outstrs.Add(outstr0);

            File.AppendAllLines(OutputPath, outstrs, Encoding.UTF8);


            var top10 = bestWords.OrderByDescending(e => e.Length).ToList();

            foreach (var t in top10)
            {
                Console.WriteLine(t);
                File.AppendAllText(OutputPath, t + "\n",Encoding.UTF8);
            }
        }
    }
}