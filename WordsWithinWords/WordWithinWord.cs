using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithinWords
{

   

   public class WordWithinWord
    {
        public string Word { get; set; }

        public List<string> WordsWithinWord = new List<string>();

        public List<WordWithinWord> WordsWithinWordsRecursive = new List<WordWithinWord>();

        public List<string> NotWords = new List<string>();

        public Dictionary<string,WordWithinWord> WordDictionary = new Dictionary<string, WordWithinWord>();

        public bool HasAny => WordsWithinWord.Any();

        public bool HasAll => WordsWithinWord.Any() && !NotWords.Any();

        public int Depth
        {
            get
            {
                // Completely empty menu (not even any straight items). 0 depth.
                if (WordsWithinWordsRecursive.Count == 0)
                {
                    return 0;
                }
                // We've either got items (which would give us a depth of 1) or
                // items and groups, so find the maximum depth of any subgroups,
                // and add 1.
                return WordsWithinWordsRecursive.OfType<WordWithinWord>()
                           .Select(x => x.Depth)
                           .DefaultIfEmpty() // 0 if we have no subgroups
                           .Max() + 1;
            }
        }

        public List<string> GetWordChain()
        {
            var mostDeepWord = this;

            var d = mostDeepWord.Depth;

            var deepWords = new List<string>();
            deepWords.Add(mostDeepWord.Word);
            while (d > 0)
            {
                var subdeep = mostDeepWord.WordsWithinWordsRecursive.Where(e => e.Depth > 0).OrderByDescending(e => e.Depth).FirstOrDefault();

                if (subdeep == null)
                {
                    d = 0;
                    break;
                }

                mostDeepWord = subdeep;
                deepWords.Add(mostDeepWord.Word);
                d = subdeep.Depth;
            }

            return deepWords;
        }

        public WordWithinWord(string word,HashSet<string> wordSet)
        {
            Word = word;
            for (var i = 0; i < word.Length; i++)
            {
                var newWord = word.Remove(i, 1);
                var exists = wordSet.Contains(newWord);

                if (exists)
                {
                    WordsWithinWord.Add(newWord);
                }
                else
                {
                    NotWords.Add(newWord);
                }              
            }            
        }        

        public string Output => Word + "\t" + string.Join(",", WordsWithinWord) + "\n";




    }
}
