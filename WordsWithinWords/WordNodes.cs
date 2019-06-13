using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WordsWithinWords
{
    public class WordNodesAndEdges
    {
        public static void Build(List<WordWithinWord> wordWithinWords, Dictionaries dictionaries)
        {
            var sw = new Stopwatch();
            sw.Start();
            var edges = new List<WordEdge>();
            wordWithinWords = wordWithinWords.OrderByDescending(e => e.Depth).Take(200).ToList();

            var nodeDict = new Dictionary<string, WordNode>();
            for (var i = 0; i < wordWithinWords.Count; i++)
            {
                var word = wordWithinWords[i];

                var node = GetNode(nodeDict, word.Word);

                foreach (var subWord in word.WordsWithinWord.Distinct())
                {
                    var subnode = GetNode(nodeDict, subWord);

                    var edge = new WordEdge { StartNode = node.ID, EndNode = subnode.ID };

                    var exists = edges.Any(e => e.StartNode == edge.StartNode && e.EndNode == edge.EndNode);

                    if (!exists)
                    {
                        edges.Add(edge);
                    }
                }

                Progress.OutputTimeRemaining(i, wordWithinWords.Count, sw, "Making nodes and edges");
            }


            var outputFilename = "outputjson.txt";

            var outputFile = Path.Combine(dictionaries.OutputDirectory, outputFilename);

            File.WriteAllText(outputFile, "nodes:[");

            var index = 0;
            foreach (var node in nodeDict)
            {
                index++;

                var languages = dictionaries.FindLanguages(node.Key);              

                var str = "{ ID: " + node.Value.ID + ", Name:\"" + node.Value.Name + "\", Languages: \"" + string.Join(",", languages) + "\"}, \n";
                File.AppendAllText(outputFile, str);
                Progress.OutputTimeRemaining(index, nodeDict.Count, sw, "Writing nodes");
            }

            index = 0;
            File.AppendAllText(outputFile, "],\n");
            sw.Restart();

            File.AppendAllText(outputFile, "edges:[");
            foreach (var edge in edges)
            {
                index++;
                var str = "{StartNode:" + edge.StartNode + ",EndNode:" + edge.EndNode + "}, \n";
                File.AppendAllText(outputFile, str);
                Progress.OutputTimeRemaining(index, nodeDict.Count, sw, "Writing edges");
            }

            File.AppendAllText(outputFile, "],\n");
        }

        private static WordNode GetNode(Dictionary<string, WordNode> nodeDict, string word)
        {
            WordNode node;
            if (nodeDict.ContainsKey(word))
            {
                node = nodeDict[word];
            }
            else
            {
                node = new WordNode { ID = nodeDict.Count + 1, Name = word };
                nodeDict.Add(word, node);
            }

            return node;
        }
    }

    public class WordNode
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class WordEdge

    {
        public int EndNode;
        public int StartNode;
    }
}