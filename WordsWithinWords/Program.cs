using System;

namespace WordsWithinWords
{
    class Program
    {
        static void Main(string[] args)
        {

            //GetAllRearrangedStrings("test");


            var wordLists = new WordLists();

            var analyser3 = Analyser.GetAnalyser(wordLists, AnalysisType.WordsWithinWords, Language.CombineAll);
            analyser3.Start();

            


            //var analyser2 = new Analyser(wordLists, AnalysisType.WordsWithinWordsRecursive, Language.CombineAll);
            //analyser2.Start();


            //var analyser = new Analyser(wordLists, AnalysisType.WordsWithinWordsRecursive,Language.English);
            //analyser.Start();


            Console.ReadLine();

        }

      
    }
}
