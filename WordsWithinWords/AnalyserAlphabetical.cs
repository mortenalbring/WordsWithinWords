using System;
using System.Collections.Generic;
using System.Linq;

namespace WordsWithinWords
{
    public class AnalyserAlphabetical : Analyser, IAnalyser
    {
        public AnalyserAlphabetical(WordLists wordLists, Language language) : base(wordLists, AnalysisType.Alphabetical, language)
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

            var top10 = bestWords.OrderByDescending(e => e.Length).Take(10).ToList();

            foreach (var t in top10)
            {
                Console.WriteLine(t);
            }


        }
    }
}
