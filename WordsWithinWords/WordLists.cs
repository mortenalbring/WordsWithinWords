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
        public List<WordList> WordList = new List<WordList>();

        public WordList WordListEnglish => this.WordList.FirstOrDefault(e => e.Language == Language.English);
        public WordLists()
        {            
         //   WordList.Add(new WordList("C:\\temp\\Words\\WordsWithinWords\\WordsWithinWords\\norsk.txt", Language.Norwegian));
            WordList.Add(new WordList("C:\\temp\\Words\\WordsWithinWords\\WordsWithinWords\\words_alpha.txt", Language.English));
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
