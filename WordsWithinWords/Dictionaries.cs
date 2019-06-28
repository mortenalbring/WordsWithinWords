using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "english-collins-scrabble.txt"), Language.EnglishCollins));
            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "english-sowpods.txt"), Language.EnglishSowpods));
            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "english.txt"), Language.EnglishGeneral));
            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "norsk.txt"), Language.Norwegian));
            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "dansk.txt"), Language.Danish));
            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "deutsch.txt"), Language.German));
            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "swiss.txt"), Language.Swiss));
            WordList.Add(new WordList(Path.Combine(baseDir, "Dictionaries", "francais.txt"), Language.French));

            var combinedEnglishWordList = MakeCombinedEnglishWordList();
            WordList.Add(combinedEnglishWordList);


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

        private WordList MakeCombinedEnglishWordList()
        {
            var totalEnglish = new HashSet<string>();
            var english1 = WordList.First(e => e.Language == Language.EnglishSowpods);
            var english2 = WordList.First(e => e.Language == Language.EnglishGeneral);

            var englishHashSets = new List<HashSet<string>>();
            englishHashSets.Add(english1.WordSet);
            englishHashSets.Add(english2.WordSet);

            foreach (var englishHashSet in englishHashSets)
            {
                foreach (var w in englishHashSet)
                {
                    if (!totalEnglish.Contains(w))
                    {
                        totalEnglish.Add(w);
                    }
                }
            }

            var engWl = new WordList(Language.EnglishCombined);
            engWl.WordSet = totalEnglish;
            return engWl;
        }

        public List<Language> FindLanguages(string word)
        {
            var output = new List<Language>();

            foreach (var w in WordList)
            {
                if (w.WordSet.Contains(word))
                {
                    output.Add(w.Language);
                }
            }

            return output;

        }

        public string OutputDirectory { get; set; }

        public string ProjectDirectory { get; set; }
        public HashSet<string> TotalHashSet { get; set; }

        public readonly List<WordList> WordList = new List<WordList>();
    }

    public enum Language
    {
        EnglishGeneral,
        EnglishSowpods,
        EnglishCollins,
        EnglishCombined,
        Norwegian,
        Danish,
        German,
        Swiss,
        French,
        CombineAll
    }
}