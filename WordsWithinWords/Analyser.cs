using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace WordsWithinWords
{
    public class Analyser
    {
        private string InputPath { get; }

        private string OutputPath { get; }

        private AnalysisType AnalysisType { get; set; }

        private HashSet<string> WordSet { get; set; }

        private List<WordWithinWord> _wordWithinWords = new List<WordWithinWord>();
        
        



        public Analyser(WordList wordList, AnalysisType analysisType)
        {
            InputPath = wordList.InputPath;

            OutputPath = InputPath + wordList.Language + analysisType + ".txt";

            AnalysisType = analysisType;

            var allWords = File.ReadAllLines(InputPath, Encoding.GetEncoding(1252));

            WordSet = new HashSet<string>();
            foreach (var w in allWords)
            {
                WordSet.Add(w);
            }
        }

        public void Start()
        {
            var sw = new Stopwatch();
            sw.Start();


            var index = 0;
            foreach (var word in WordSet)
            {
                index++;

                if (word.Length <= 2)
                {
                    continue;
                }

                switch (AnalysisType)
                {
                    case AnalysisType.WordsWithinWords:

                        var ww = new WordWithinWord(word, WordSet);
                        this._wordWithinWords.Add(ww);                      

                        break;
                    case AnalysisType.Alphabetical:
                        break;
                    case AnalysisType.SwappedLetters:
                        break;
                    case AnalysisType.WordsWithinWordsRecursive:

                        var wwr = new WordWithinWord(word, WordSet);

                        this._wordWithinWords.Add(wwr);


                        break;
                }

                //  FindWordsWithAllWords(word);
                //FindWordsSwapped(word, hs, bestWordsDictionary);
                //   FindWordsAlphabetical(word, hs, bestWordsDictionary);

                Progress.OutputTimeRemaining(index, WordSet.Count, sw);
            }

          
            sw.Stop();
            sw.Restart();
            if (AnalysisType == AnalysisType.WordsWithinWordsRecursive)
            {
                DoRecursiveWords(sw);
            }

            Console.WriteLine($"Writing output {OutputPath}");

            File.WriteAllText(OutputPath, "");
            this._wordWithinWords = this._wordWithinWords.OrderByDescending(e => e.WordsWithinWord.Count).ToList();

            foreach (var word in this._wordWithinWords)
            {
                if (word.HasAll)
                {                    
                    File.AppendAllText(OutputPath, word.Output, Encoding.UTF8);
                }

            }

            Console.WriteLine($"Done writing output {OutputPath}");



            
        }

        private void DoRecursiveWords(Stopwatch sw)
        {

            WordNodesAndEdges.Build(this._wordWithinWords);


            var wdict = new Dictionary<string, WordWithinWord>();
            foreach (var w in this._wordWithinWords)
            {
                if (w.HasAny)
                {
                    wdict.Add(w.Word, w);
                }
            }

            var i = 0;

            foreach (var w in wdict)
            {
                foreach (var s in w.Value.WordsWithinWord)
                {
                    var exists = wdict.ContainsKey(s);
                    if (exists)
                    {
                        var elem = wdict[s];
                        w.Value.WordsWithinWordsRecursive.Add(elem);
                    }
                }

                i++;

                Progress.OutputTimeRemaining(i, wdict.Count, sw);
            }

            var wdepthList = wdict.Values.Where(e => e.Depth > 0).Select(e => e).OrderByDescending(e => e.Depth).ToList();

            foreach (var word in wdepthList)
            {
                var wordChain = word.GetWordChain();

                Console.WriteLine(word.Word + "\t" + string.Join(",", wordChain));
            }

         

            foreach (var w in wdepthList)
            {
                Console.WriteLine(w.Output);
                Console.WriteLine(w.WordsWithinWordsRecursive.Count);
                Console.WriteLine(w.Depth);
            }
        }

        private static List<string> GetWordChain(WordWithinWord mostDeepWord)
        {
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

       

    }
}