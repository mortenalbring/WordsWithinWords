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

            var edges = new HashSet<WordEdge>();

            wordWithinWords = wordWithinWords.Where(e => e.Depth > 2).OrderByDescending(e => e.Depth).ToList();

            var nodeDict = new Dictionary<string, WordNode>();
            for (var i = 0; i < wordWithinWords.Count; i++)
            {
                var word = wordWithinWords[i];
                var node = GetNode(nodeDict, word.Word);
                var distinctWords = word.WordsWithinWord.Distinct().ToList();
                foreach (var subWord in distinctWords)
                {
                    var subNode = GetNode(nodeDict, subWord);
                    var edge = new WordEdge { StartNode = node.ID, EndNode = subNode.ID };

                    var exists = edges.Any(e => e.StartNode == edge.StartNode && e.EndNode == edge.EndNode);
                    if (!exists)
                    {
                        edges.Add(edge);
                    }
                }

                Progress.OutputTimeRemaining(i, wordWithinWords.Count, sw, "Making nodes and edges");
            }


            var mostConnectedA = edges.GroupBy(e => e.EndNode).OrderByDescending(e => e.Count()).Take(50).ToList();
            var mostConnectedB = edges.GroupBy(e => e.StartNode).OrderByDescending(e => e.Count()).Take(50).ToList();

            var interestingNodes = new Dictionary<string,WordNode>();
            var interestingEdges = new HashSet<WordEdge>();

            foreach (var m in mostConnectedA)
            {
                var endNode = m.Key;
                var startNodes = m.ToList();

                foreach (var node in startNodes)
                {
                    var wnode = nodeDict.FirstOrDefault(e => e.Value.ID == node.StartNode);

                    if (wnode.Key == "chores")
                    {
                        var xxx = 42;
                    }

                    if (!interestingNodes.ContainsKey(wnode.Key))
                    {
                        var relevantEdges = edges.Where(e => e.StartNode == wnode.Value.ID || e.EndNode == wnode.Value.ID).ToList();

                        if (relevantEdges.Count > 0)
                        {
                            interestingNodes.Add(wnode.Key, wnode.Value);

                            foreach (var relevantEdge in relevantEdges)
                            {
                                var relevantNode = nodeDict.FirstOrDefault(e => e.Value.ID == relevantEdge.StartNode);
                                if (!interestingNodes.ContainsKey(relevantNode.Key))
                                {
                                    interestingNodes.Add(relevantNode.Key,relevantNode.Value);
                                }

                                var relevantNode2 = nodeDict.FirstOrDefault(e => e.Value.ID == relevantEdge.EndNode);
                                if (!interestingNodes.ContainsKey(relevantNode2.Key))
                                {
                                    interestingNodes.Add(relevantNode2.Key, relevantNode2.Value);
                                }


                                if (!interestingEdges.Contains(relevantEdge))
                                {
                                    interestingEdges.Add(relevantEdge);
                                }
                            }
                        }
                        else
                        {
                            var xx = 42;
                        }

                    }
                }
            }

            //WriteJsonOutput(dictionaries, outputFile, nodeDict, edges);
            WriteJsonOutput(dictionaries, outputFile, interestingNodes, interestingEdges);
        }

        private static void WriteJsonOutput(Dictionaries dictionaries, string outputFile, Dictionary<string, WordNode> nodeDict, HashSet<WordEdge> edgeshs)
        {
            var sw = new Stopwatch();
            sw.Start();

            File.WriteAllText(outputFile, "{\"nodes\":[");
            var index = 0;
            var nodeIndex = new Dictionary<string, int>();
            foreach (var node in nodeDict)
            {
                nodeIndex.Add(node.Key, index);
                index++;
                var languages = dictionaries.FindLanguages(node.Key);

                var str = "{ \"ID\": " + node.Value.ID + ", \"name\":\"" + node.Value.Name + "\", \"languages\": \"" + string.Join(",", languages) + "\"}";

                if (index < nodeDict.Count)
                {
                    str += ",";
                }
                str += "\n";


                File.AppendAllText(outputFile, str);
                Progress.OutputTimeRemaining(index, nodeDict.Count, sw, "Writing nodes");
            }

            index = 0;
            File.AppendAllText(outputFile, "],\n");


            File.AppendAllText(outputFile, "\"links\":[");
            foreach (var edge in edgeshs)
            {
                index++;

                var startNode = nodeDict.Values.FirstOrDefault(e => e.ID == edge.StartNode);
                var endNode = nodeDict.Values.FirstOrDefault(e => e.ID == edge.EndNode);

                if (startNode is null)
                {
                    continue;
                }
                if (endNode is null) 
                {
                    continue;
                }

                var startNodeIndx = nodeIndex[startNode.Name];
                var endNodeIndx = nodeIndex[endNode.Name];

                var str2 = "{\"source\":" + startNodeIndx + ",\"target\":" + endNodeIndx + "}";

                if (index < edgeshs.Count)
                {
                    str2 += ",";
                }
                str2 += "\n";

                File.AppendAllText(outputFile, str2);
                Progress.OutputTimeRemaining(index, nodeDict.Count, sw, "Writing edges");
            }

            File.AppendAllText(outputFile, "]\n }");
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