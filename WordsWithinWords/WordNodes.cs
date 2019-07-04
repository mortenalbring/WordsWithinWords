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


            wordWithinWords = wordWithinWords.Where(e => e.Depth > 2).OrderByDescending(e => e.Depth).Take(10).ToList();

            var nodeDict = new Dictionary<string, WordNode>();

            var output = MakeWordOutput(wordWithinWords, nodeDict, sw);

            var filteredOutput = FilterOutput(output);





            WriteJsonOutput(outputFile, output);
            //WriteJsonOutput(dictionaries, outputFile, nodeDict, edges);
            //WriteJsonOutput(dictionaries, outputFile, interestingNodes, interestingEdges);
        }

        private static WordOutput FilterOutput(WordOutput output)
        {
            var filteredOutput = new WordOutput();


            var mostConnectedEdges = output.Edges.GroupBy(e => e.StartNode).OrderByDescending(e => e.Count()).Take(50).ToList();

            foreach (var edge in mostConnectedEdges)
            {
                var node = new WordNode();
                node.Name = edge.Key;

                AddNode(filteredOutput, node);

                foreach (var subedge in edge)
                {
                    var subnode = new WordNode();
                    node.Name = subedge.EndNode;

                    AddNode(filteredOutput, subnode);


                    AddEdge(filteredOutput, subedge);
                }

            }


            return filteredOutput;

        }

        private static void AddEdge(WordOutput filteredOutput, WordEdge subedge)
        {
            if (filteredOutput.Edges is null)
            {
                filteredOutput.Edges = new HashSet<WordEdge>();
            }
            if (!filteredOutput.Edges.Contains(subedge))
            {
                filteredOutput.Edges.Add(subedge);
            }
        }

        private static void AddNode(WordOutput filteredOutput, WordNode node)
        {
            if (filteredOutput.Nodes is null)
            {
                filteredOutput.Nodes = new HashSet<WordNode>();
            }
            if (!filteredOutput.Nodes.Contains(node))
            {
                filteredOutput.Nodes.Add(node);
            }
        }

        private static WordOutput MakeWordOutput(List<WordWithinWord> wordWithinWords, Dictionary<string, WordNode> nodeDict, Stopwatch sw)
        {
            var edges2 = new HashSet<WordEdge>();
            var nodes2 = new HashSet<WordNode>();


            for (var i = 0; i < wordWithinWords.Count; i++)
            {
                var word = wordWithinWords[i];

                var node = new WordNode();
                node.Name = word.Word;

                if (!nodes2.Contains(node))
                {
                    nodes2.Add(node);
                }

                var distinctWords = word.WordsWithinWord.Distinct().ToList();

                foreach (var subWord in distinctWords)
                {
                    var subNode = new WordNode();
                    subNode.Name = subWord;


                    if (!nodes2.Contains(subNode))
                    {
                        nodes2.Add(subNode);
                    }

                    var edge2 = new WordEdge { StartNode = node.Name, EndNode = subNode.Name };


                    var exists2 = edges2.Any(e => e.StartNode == edge2.StartNode && e.EndNode == edge2.EndNode);
                    if (!exists2)
                    {
                        edges2.Add(edge2);
                    }
                }


                Progress.OutputTimeRemaining(i, wordWithinWords.Count, sw, "Making nodes and edges");
            }

            var output = new WordOutput { Nodes = nodes2, Edges = edges2 };

            return output;
        }

        private static void WriteJsonOutput(string outputFile, WordOutput wordOutput)
        {
            var sw = new Stopwatch();
            sw.Start();

            File.WriteAllText(outputFile, "{\"nodes\":[");
            var index = 0;
            var nodeIndex = new Dictionary<string, int>();
            foreach (var node in wordOutput.Nodes)
            {

                index++;


                var str = "{ \"id\": \"" + node.Name + "\", \"group\":1}";

                if (index < wordOutput.Nodes.Count)
                {
                    str += ",";
                }
                str += "\n";


                File.AppendAllText(outputFile, str);
                Progress.OutputTimeRemaining(index, wordOutput.Nodes.Count, sw, "Writing nodes");
            }

            index = 0;
            File.AppendAllText(outputFile, "],\n");


            File.AppendAllText(outputFile, "\"links\":[");
            foreach (var edge in wordOutput.Edges)
            {
                index++;

                var str2 = "{\"source\":\"" + edge.StartNode + "\",\"target\":\"" + edge.EndNode + "\",\"value\":1}";

                if (index < wordOutput.Edges.Count)
                {
                    str2 += ",";
                }
                str2 += "\n";

                File.AppendAllText(outputFile, str2);
                Progress.OutputTimeRemaining(index, wordOutput.Edges.Count, sw, "Writing edges");
            }

            File.AppendAllText(outputFile, "]\n }");

        }



    }

    public class WordNode
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public string Group { get; set; }
    }



    public class WordEdge
    {
        public string StartNode { get; set; }

        public string EndNode { get; set; }
    }

    public class WordOutput
    {
        public HashSet<WordNode> Nodes { get; set; }
        public HashSet<WordEdge> Edges { get; set; }
    }
}