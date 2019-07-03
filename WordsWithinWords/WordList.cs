using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WordsWithinWords
{
    public class WordList
    {
        public WordList(Language language)
        {
            Language = language;

        }
        public WordList(string inputPath, Language language)
        {
            InputPath = inputPath;
            Language = language;

            var allWords = File.ReadAllLines(InputPath, Encoding.UTF8);

            WordSet = new HashSet<string>();
            foreach (var w in allWords)
            {
                if (w.Contains(" "))
                {
                    continue;
                }

                if (WordSet.Contains(w.ToLower()))
                {
                    continue;
                }

                WordSet.Add(w.ToLower());
            }
        }

        public string InputPath { get; set; }
        public Language Language { get; set; }

        public HashSet<string> WordSet { get; set; }
    }
}