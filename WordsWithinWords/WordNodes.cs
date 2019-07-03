using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WordsWithinWords
{
    public class WordNodesAndEdges
    {
        public static void Build(Language language, List<WordWithinWord> wordWithinWords, Dictionaries dictionaries)
        {
            var outputFilename = language.ToString() + "WordNodesRecursiveJson.json";
            var outputFile = Path.Combine(dictionaries.OutputDirectory, outputFilename);

            var sw = new Stopwatch();
            sw.Start();
            var edges = new List<WordEdge>();
            wordWithinWords = wordWithinWords.Where(e => e.Depth > 2).OrderByDescending(e => e.Depth).ToList();

            var nodeDict = new Dictionary<string, WordNode>();
            for (var i = 0; i < wordWithinWords.Count; i++)
            {
                var word = wordWithinWords[i];

                var node = GetNode(nodeDict, word.Word);
                var distinctWords = word.WordsWithinWord.Distinct().ToList();
                foreach (var subWord in distinctWords)
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


            var mostConnected = edges.GroupBy(e => e.EndNode).OrderByDescending(e => e.Count());

            File.WriteAllText(outputFile, "{\"nodes\":[");

            var index = 0;

            var nodeIndex = new Dictionary<string,int>();
            foreach (var node in nodeDict)
            {
                
                nodeIndex.Add(node.Key,index);
                index++;
                var languages = dictionaries.FindLanguages(node.Key);              

                var str = "{ \"ID\": " + node.Value.ID + ", \"name\":\"" + node.Value.Name + "\", \"languages\": \"" + string.Join(",", languages) + "\"}, \n";
                File.AppendAllText(outputFile, str);
                Progress.OutputTimeRemaining(index, nodeDict.Count, sw, "Writing nodes");
            }

            index = 0;
            File.AppendAllText(outputFile, "],\n");
            sw.Restart();

            File.AppendAllText(outputFile, "\"links\":[");
            foreach (var edge in edges)
            {
                index++;
                var str = "{StartNode:" + edge.StartNode + ",EndNode:" + edge.EndNode + "}, \n";

                var startNode = nodeDict.Values.FirstOrDefault(e => e.ID == edge.StartNode);
                var endNode = nodeDict.Values.FirstOrDefault(e => e.ID == edge.EndNode);

                var startNodeIndx = nodeIndex[startNode.Name];
                var endNodeIndx = nodeIndex[endNode.Name];

                var str2 = "{\"source\":" + startNodeIndx + ",\"target\":" + endNodeIndx + "}, \n";


                File.AppendAllText(outputFile, str2);
                Progress.OutputTimeRemaining(index, nodeDict.Count, sw, "Writing edges");
            }

            File.AppendAllText(outputFile, "],\n }");
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