using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WordsWithinWords
{
    public class Dictionaries
    {
        public Dictionaries()
        {
            var workingDirectory = Environment.CurrentDirectory;
            var baseDir = Directory.GetParent(workingDirectory).Parent.FullName;

            ProjectDirectory = baseDir;

            OutputDirectory = Path.Combine(ProjectDirectory, "Output");

            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "english.txt"), Language.English));
            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "norsk.txt"), Language.Norwegian));
            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "dansk.txt"), Language.Danish));
            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "deutsch.txt"), Language.German));
            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "swiss.txt"), Language.Swiss));

            var totalHs = new HashSet<string>();
            foreach (var wl in WordList)
            {
                foreach (var hs in wl.WordSet)
                {
                    if (!totalHs.Contains(hs))
                    {
                        totalHs.Add(hs);
                    }
                }
            }

            TotalHashSet = totalHs;
        }

        public string OutputDirectory { get; set; }

        public string ProjectDirectory { get; set; }
        public HashSet<string> TotalHashSet { get; set; }

        public WordList WordListEnglish => WordList.FirstOrDefault(e => e.Language == Language.English);

        public List<WordList> WordList = new List<WordList>();
    }

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

    public enum Language
    {
        English,
        Norwegian,
        Danish,
        German,
        Swiss,
        CombineAll
    }
}