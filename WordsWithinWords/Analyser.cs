using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WordsWithinWords
{
    public class Analyser
    {
        protected Analyser(Dictionaries dictionaries, AnalysisType analysisType, Language language)
        {
            Dictionaries = dictionaries;
            AnalysisType = analysisType;
            Language = language;
            var outputFilename = Language.ToString() + analysisType + ".txt";
            OutputPath = Path.Combine(dictionaries.OutputDirectory, outputFilename);

            if (language == Language.CombineAll)
            {
                WordSet = dictionaries.TotalHashSet;
            }
            else
            {
                var wordList = dictionaries.WordList.First(e => e.Language == language);

                WordSet = wordList.WordSet;
            }
        }

        private AnalysisType AnalysisType { get; }


        protected Dictionaries Dictionaries { get; set; }

        private Language Language { get; }
        protected string OutputPath { get; }

        protected HashSet<string> WordSet { get; set; }

        protected Stopwatch Sw = new Stopwatch();

        public static IAnalyser GetAnalyser(Dictionaries wordLists, AnalysisType analysisType, Language language)
        {
            switch (analysisType)
            {
                case AnalysisType.WordsWithinWords:
                    return new AnalyserWordWithinWord(wordLists, language);
                case AnalysisType.WordsWithinWordsRecursive:
                    return new AnalyserRecursive(wordLists, language);
                case AnalysisType.Alphabetical:
                    return new AnalyserAlphabetical(wordLists, language);
                case AnalysisType.SwappedLetters:
                    return new AnalyserSwapper(wordLists, language);
            }

            return null;
        }
    }
}