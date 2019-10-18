using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WordsWithinWords.Analysers
{
    public class AnalyserDistribution : Analyser, IAnalyser
    {


        public void Start()
        {
            var lengthDists = new Dictionary<int,int>();

            var longestWordLength = WordSet.Max(e => e.Length);

            for (int i = 1; i <= longestWordLength; i++)
            {
                lengthDists.Add(i,0);
            }

            foreach (var word in WordSet)
            {
                var wordLength = word.Length;
                lengthDists[wordLength]++;
            }

            File.WriteAllText(OutputPath,"");

            var longestWords = WordSet.Where(e => e.Length == longestWordLength).ToList();

            foreach (var w in longestWords)
            {
                File.AppendAllText(OutputPath,w.Length + "\t" + w + "\n");
            }

            foreach (var elem in lengthDists)
            {
                var percentage = ((float)elem.Value / WordSet.Count) * 100;


                File.AppendAllText(OutputPath,"\"" + elem.Key + "," + elem.Value + "," + percentage + "\",\n");
            }

        }

        public AnalyserDistribution(Dictionaries dictionaries, Language language) : base(dictionaries, AnalysisType.LengthDistribution, language)
        {
        }
    }
}
