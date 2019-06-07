using System;

namespace WordsWithinWords
{
    class Program
    {
        static void Main(string[] args)
        {

            //GetAllRearrangedStrings("test");


            var wordLists = new Dictionaries();

            var analyser3 = Analyser.GetAnalyser(wordLists, AnalysisType.WordsWithinWords, Language.CombineAll);
            analyser3.Start();

            
            Console.ReadLine();

        }

      
    }
}
