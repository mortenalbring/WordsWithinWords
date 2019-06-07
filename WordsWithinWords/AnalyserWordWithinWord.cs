using System;
using System.IO;
using System.Linq;
using System.Text;

namespace WordsWithinWords
{
    public class AnalyserWordWithinWord : Analyser, IAnalyser
    {
        public AnalyserWordWithinWord(WordLists wordLists, Language language) : base(wordLists, AnalysisType.WordsWithinWords, language)
        {
            
        }

        public void Start()
        {
            foreach (var word in WordSet)
            {
                var ww = new WordWithinWord(word,WordSet);
                this.WordWithinWords.Add(ww);

            }

            Console.WriteLine($"Writing output {OutputPath}");

            File.WriteAllText(OutputPath, "");
            this.WordWithinWords = this.WordWithinWords.OrderByDescending(e => e.WordsWithinWord.Count).ToList();

            foreach (var word in this.WordWithinWords)
            {
                if (word.HasAll)
                {
                    File.AppendAllText(OutputPath, word.Output, Encoding.UTF8);
                }

            }

            Console.WriteLine($"Done writing output {OutputPath}");

        }
    }
}
