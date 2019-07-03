using System;

namespace WordsWithinWords
{
    internal class Program
    {
        private static void Main(string[] args)
        {
        //    RunAll();

            //GetAllRearrangedStrings("test");


            var wordLists = new Dictionaries();

            
            
            //foreach (var l in wordLists.WordList)
            //{
            //    var analyser3 = Analyser.GetAnalyser(wordLists, AnalysisType.WordsWithinWords, l.Language);
            //    analyser3.Start();
            //}

            //Analyser.GetAnalyser(wordLists, AnalysisType.Alphabetical, Language.EnglishGeneral).Start();
            Analyser.GetAnalyser(wordLists, AnalysisType.WordsWithinWordsRecursive, Language.EnglishSowpods).Start();

            Console.WriteLine("All done");
            Console.ReadLine();
        }

        private static void RunAll()
        {
            var wordLists = new Dictionaries();

            var analysisTypes = Enum.GetValues(typeof(AnalysisType));

            foreach(AnalysisType analysisType in analysisTypes)
            {
                var languages = Enum.GetValues(typeof(Language));

                foreach(Language language in languages)
                {
                    Console.WriteLine(analysisType.ToString() + " " + language.ToString());

                    Analyser.GetAnalyser(wordLists, analysisType, language).Start();
                }

            }

        }
    }
}