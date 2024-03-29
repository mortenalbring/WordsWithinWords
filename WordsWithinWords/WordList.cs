﻿using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WordsWithinWords
{
    public class WordList
    {
        public WordList(string inputPath, Language language)
        {
            InputPath = inputPath;
            Language = language;

            var allWords = File.ReadAllLines(InputPath, Encoding.GetEncoding(1252));

            WordSet = new HashSet<string>();
            foreach (var w in allWords)
            {
                WordSet.Add(w.ToLower());
            }
        }

        public string InputPath { get; set; }
        public Language Language { get; set; }

        public HashSet<string> WordSet { get; set; }
    }
}