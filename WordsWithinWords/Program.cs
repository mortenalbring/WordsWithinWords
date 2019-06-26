﻿using System;

namespace WordsWithinWords
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //GetAllRearrangedStrings("test");


            var wordLists = new Dictionaries();

            
            
            //foreach (var l in wordLists.WordList)
            //{
            //    var analyser3 = Analyser.GetAnalyser(wordLists, AnalysisType.WordsWithinWords, l.Language);
            //    analyser3.Start();
            //}

            var analyser4 = Analyser.GetAnalyser(wordLists, AnalysisType.WordsWithinWords, Language.English);

            analyser4.Start();

            Console.WriteLine("All done");
            Console.ReadLine();
        }
    }
}