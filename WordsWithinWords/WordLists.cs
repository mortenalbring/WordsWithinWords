using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithinWords
{
    public class WordLists
    {
        public HashSet<string> TotalHashSet { get; set; }

        public List<WordList> WordList = new List<WordList>();

        public WordList WordListEnglish => this.WordList.FirstOrDefault(e => e.Language == Language.English);
        public WordLists()
        {


            

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

         //   WordList.Add(new WordList("C:\\temp\\Words\\WordsWithinWords\\WordsWithinWords\\norsk.txt", Language.Norwegian));
            WordList.Add(new WordList(Path.Combine(baseDir,"words_alpha.txt"), Language.English));
            WordList.Add(new WordList(Path.Combine(baseDir, "norsk.txt"), Language.Norwegian));

            var totalHs = new HashSet<string>();
            foreach(var wl in WordList)
            {
                foreach(var hs in wl.WordSet)
                {
                    if (!totalHs.Contains(hs))
                    {
                        totalHs.Add(hs);
                    }
                }
            }

            TotalHashSet = totalHs;
        }
    }

    public class WordList
    {
        public string InputPath { get; set; }        
        public Language Language { get; set; }       

        public HashSet<string> WordSet { get; set; }

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
    }

    public enum Language
    {
        English,
        Norwegian
    }
}
