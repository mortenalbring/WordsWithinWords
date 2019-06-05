using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace WordsWithinWords
{

    public class WordNodesAndEdges
    {

        public static void Build(List<WordWithinWord> wordWithinWords)
        {
            var sw = new Stopwatch();
            sw.Start();
            var edges = new List<WordEdge>();
            wordWithinWords = wordWithinWords.OrderByDescending(e => e.Depth).Take(1000).ToList();

            var nodeDict = new Dictionary<string,WordNode>();
            for (var i = 0; i < wordWithinWords.Count; i++)
            {
                var word = wordWithinWords[i];
              
                var node = GetNode(nodeDict, word.Word);

                foreach (var subWord in word.WordsWithinWord)
                {
                    var subnode = GetNode(nodeDict, subWord);
                    var edge = new WordEdge();
                    edge.StartNode = node.ID;
                    edge.EndNode = subnode.ID;

                    var exists = edges.Any(e => e.StartNode == edge.StartNode && e.EndNode == edge.EndNode);
                    if (!exists)
                    {
                        edges.Add(edge);
                    }
                }
                Progress.OutputTimeRemaining(i, wordWithinWords.Count, sw, "Making nodes and edges");
            }


            var outputFile = "outputjson.txt";

            File.WriteAllText(outputFile,"nodes:[");
           
            var index = 0;
            foreach (var node in nodeDict)
            {
                index++;
                var str = "{ ID: " + node.Value.ID + ", Name:\"" + node.Value.Name + "\"}, \n";
                File.AppendAllText(outputFile, str);                
                Progress.OutputTimeRemaining(index, nodeDict.Count, sw,"Writing nodes");
            }

            index = 0;
            File.AppendAllText(outputFile, "],\n");
            sw.Restart();

            File.AppendAllText(outputFile,"edges:[");
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
                node = new WordNode();
                node.ID = nodeDict.Count + 1;
                node.Name = word;
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
        public int StartNode;
        public int EndNode;
    }
}
