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
            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "francais.txt"), Language.French));

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

            var totalHs2 = new HashSet<Word>();
            foreach (var wl in WordList)
            {
                foreach (var hs in wl.WordSet2)
                {
                    if (totalHs2.Contains(hs))
                    {
                        //do something?
                    }

                }
            }

        }

        public string OutputDirectory { get; set; }

        public string ProjectDirectory { get; set; }
        public HashSet<string> TotalHashSet { get; set; }
        public HashSet<Word> TotalHashSet2 { get; set; }

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
            WordSet2 = new HashSet<Word>();
            foreach (var w in allWords)
            {
                var text = w.ToLower();
                var word = new Word();
                word.Languages.Add(language);
                word.Text = text;
                WordSet2.Add(word);
                WordSet.Add(w.ToLower());
            }
        }

        public string InputPath { get; set; }
        public Language Language { get; set; }

        public HashSet<string> WordSet { get; set; }
        public HashSet<Word> WordSet2 { get; set; }
    }

    public class Word
    {
        public string Text { get; set; }
        public List<Language> Languages = new List<Language>();

        public override int GetHashCode()
        {
            return this.Text.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Text.Equals(((Word)obj).Text);
        }
    }

    public enum Language
    {
        English,
        Norwegian,
        Danish,
        German,
        Swiss,
        French,
        CombineAll
    }
}