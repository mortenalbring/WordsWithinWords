using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace WordsWithinWords
{
    class Program
    {
        static void Main(string[] args)
        {

            //GetAllRearrangedStrings("test");
       
         
            var wordLists = new WordLists();

            foreach (var w in wordLists.WordList)
            {
                var analyser = new Analyser(w, AnalysisType.WordsWithinWordsRecursive);
                analyser.Start();

            }


        }

        private static void FindWordsAlphabetical(string word, HashSet<string> hs, Dictionary<string, List<string>> bestWordsDictionary)
        {
            var chars = word.ToList().OrderBy(e => e).ToList();

            var alphaword = "";
            foreach (var c in chars)
            {
                alphaword = alphaword + c;
            }

            if (alphaword == word)
            {
                bestWordsDictionary.Add(word,new List<string>());
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

            for (int i = 0; i < chars.Count; i++)
            {                
                for (int j = 0; j < chars.Count; j++)
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

            for (int i = 0; i < word.Length; i++)
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
