﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace WordsWithinWords
{
    public class AnalyserSwapper : Analyser, IAnalyser
    {
        public AnalyserSwapper(Dictionaries dictionaries, Language language) : base(dictionaries, AnalysisType.SwappedLetters, language)
        {
        }

        public void Start()
        {
            Sw.Restart();
            var bestWords = new Dictionary<string, List<string>>();

            var index = 0;
            foreach (var word in WordSet)
            {
                index++;
                FindWordsSwapped(word, WordSet, bestWords);

                Progress.OutputTimeRemaining(index, WordSet.Count, Sw);
            }

            //bestWords = bestWords.OrderByDescending(e => e.Key.Length).ToDictionary(e => e.Key, e => e.Value);
            bestWords = bestWords.OrderByDescending(e => e.Value.Count).ToDictionary(e => e.Key, e => e.Value);


            foreach (var w in bestWords)
            {
                Console.WriteLine(w.Key);
                foreach (var ww in w.Value)
                {
                    Console.Write(ww + ",");
                }
            }
        }

        private static void FindWordsSwapped(string word, HashSet<string> hs, Dictionary<string, List<string>> bestWordsDictionary)
        {
            var allRearrangedWords = GetAllRearrangedStrings(word);
            var success = true;

            var wordList = new List<string>();

            foreach (var newWord in allRearrangedWords)
            {
                wordList.Add(newWord);
                var exists = hs.Contains(newWord);
                if (!exists)
                {
                    success = false;
                }
            }

            if (success)
            {
                bestWordsDictionary.Add(word, wordList);
            }
        }

        private static List<string> GetAllRearrangedStrings(string word)
        {
            var output = new List<string>();

            var chars = word.ToList();

            for (var i = 0; i < chars.Count; i++)
            {
                for (var j = 0; j < chars.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var newword = SwapChars(word, i, j);
                    output.Add(newword);
                }
            }


            return output;
        }

        private static string SwapChars(string word, int from, int to)
        {
            var newWord = "";

            for (var i = 0; i < word.Length; i++)
            {
                var wordchar = word[i];
                if (i == from)
                {
                    wordchar = word[to];
                }

                if (i == to)
                {
                    wordchar = word[from];
                }

                newWord = newWord + wordchar;
            }

            return newWord;
        }
    }
}