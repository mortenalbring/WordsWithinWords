using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WordsWithinWords
{
    public class Analyser 
    {
        private string InputPath { get; }

        protected string OutputPath { get; }

        private AnalysisType AnalysisType { get; set; }

        protected HashSet<string> WordSet { get; set; }

        protected List<WordWithinWord> WordWithinWords = new List<WordWithinWord>();

        protected WordLists WordLists { get; set; }

        private Language Language { get; set; }

        protected Stopwatch Sw = new Stopwatch();

        protected Analyser(WordLists wordLists, AnalysisType analysisType, Language language)
        {
            AnalysisType = analysisType;
            Language = language;
            OutputPath = InputPath + Language + analysisType + ".txt";

            if (language == Language.CombineAll)
            {
                WordSet = wordLists.TotalHashSet;
            }
            else
            {
                var wordList = wordLists.WordList.First(e => e.Language == language);

                WordSet = wordList.WordSet;
            }
        }

        public static IAnalyser GetAnalyser(WordLists wordLists, AnalysisType analysisType, Language language)
        {
            switch (analysisType)
            {
                case AnalysisType.WordsWithinWords:
                    return new AnalyserWordWithinWord(wordLists, language);
                    break;
                case AnalysisType.WordsWithinWordsRecursive:
                    return new AnalyserRecursive(wordLists, language);
                    break;
                case AnalysisType.Alphabetical:
                    return new AnalyserAlphabetical(wordLists, language);
                    break;
                case AnalysisType.SwappedLetters:
                    return new AnalyserSwapper(wordLists, language);
                    break;
            }

            return null;
        }

    }
}