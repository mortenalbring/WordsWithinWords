using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace WordsWithinWords
{
    public class WordNodesAndEdges
    {
        public static void Build(Language language, List<WordWithinWord> wordWithinWords, Dictionaries dictionaries, string filename)
        {
            var outputFilename = language.ToString() + filename + ".json";
            var xmlFilename = language + filename + "Serial.xml";
            var outputFile = Path.Combine(dictionaries.OutputDirectory, outputFilename);
            var xmlOutput = Path.Combine(dictionaries.OutputDirectory, xmlFilename);
            
            var sw = new Stopwatch();
            sw.Start();
            
            wordWithinWords = wordWithinWords.Where(e => e.Word.Length > 5).ToList();

          //  wordWithinWords = wordWithinWords.Where(e => e.Depth > 2).OrderByDescending(e => e.Depth).Take(10).ToList();

            var nodeDict = new Dictionary<string, WordNode>();

            var wordOutput = MakeWordOutput(wordWithinWords, nodeDict, sw);

            var sh = new SerialHelper();
            sh.SerializeObject(wordOutput,xmlOutput);
            
            var filteredOutput = FilterOutput(wordOutput);





            WriteJsonOutput(outputFile, wordOutput);
            //WriteJsonOutput(dictionaries, outputFile, nodeDict, edges);
            //WriteJsonOutput(dictionaries, outputFile, interestingNodes, interestingEdges);
        }

        private static WordOutput FilterOutput(WordOutput output)
        {
            var filteredOutput = new WordOutput();


            var mostConnectedEdges = output.links.GroupBy(e => e.source).OrderByDescending(e => e.Count()).Take(50).ToList();

            foreach (var edge in mostConnectedEdges)
            {
                var node = new WordNode();
                node.id = edge.Key;

                AddNode(filteredOutput, node);

                foreach (var subedge in edge)
                {
                    var subnode = new WordNode();
                    node.id = subedge.target;

                    AddNode(filteredOutput, subnode);


                    AddEdge(filteredOutput, subedge);
                }

            }


            return filteredOutput;

        }

        private static void AddEdge(WordOutput filteredOutput, WordEdge subedge)
        {
            if (filteredOutput.links is null)
            {
                filteredOutput.links = new HashSet<WordEdge>();
            }
            if (!filteredOutput.links.Contains(subedge))
            {
                filteredOutput.links.Add(subedge);
            }
        }

        private static void AddNode(WordOutput filteredOutput, WordNode node)
        {
            if (filteredOutput.nodes is null)
            {
                filteredOutput.nodes = new HashSet<WordNode>();
            }
            if (!filteredOutput.nodes.Contains(node))
            {
                filteredOutput.nodes.Add(node);
            }
        }

        private static WordOutput MakeWordOutput(List<WordWithinWord> wordWithinWords, Dictionary<string, WordNode> nodeDict, Stopwatch sw)
        {
            var edgeSet = new HashSet<WordEdge>();
            var nodesSet = new HashSet<WordNode>();
            
            for (var i = 0; i < wordWithinWords.Count; i++)
            {
                var word = wordWithinWords[i];

                var node = new WordNode();
                
                var exists = nodesSet.FirstOrDefault(e => e.id == word.Word);
                if (exists != null)
                {
                    node = exists;
                }
                else
                {
                    node.id = word.Word;    
                }
                
                if (!nodesSet.Contains(node))
                {
                    nodesSet.Add(node);
                }

                var distinctWords = word.WordsWithinWord.Distinct().ToList();

                foreach (var subWord in distinctWords)
                {
                    var subNode = new WordNode();
                   
                    var subexists = nodesSet.FirstOrDefault(e => e.id == subWord);
                    if (subexists != null)
                    {
                        subNode = subexists;
                    }
                    else
                    {
                        subNode.id = subWord;    
                    }

                    if (!nodesSet.Contains(subNode))
                    {
                        nodesSet.Add(subNode);
                    }

                    var edge2 = new WordEdge { source = node.id, target = subNode.id };


                    var exists2 = edgeSet.Any(e => e.source == edge2.source && e.target == edge2.target);
                    if (!exists2)
                    {
                        edgeSet.Add(edge2);
                    }
                }


                Progress.OutputTimeRemaining(i, wordWithinWords.Count, sw, "Making nodes and edges");
            }

            var output = new WordOutput { nodes = nodesSet, links = edgeSet };

            return output;
        }

        private static void WriteJsonOutput(string outputFile, WordOutput wordOutput)
        {
            var sw = new Stopwatch();
            sw.Start();

            File.WriteAllText(outputFile, "{\"nodes\":[");
            var index = 0;
            var nodeIndex = new Dictionary<string, int>();
            foreach (var node in wordOutput.nodes)
            {
                index++;
                
                var str = "{ \"id\": \"" + node.id + "\", \"group\":1}";

                if (index < wordOutput.nodes.Count)
                {
                    str += ",";
                }
                str += "\n";


                File.AppendAllText(outputFile, str);
                Progress.OutputTimeRemaining(index, wordOutput.nodes.Count, sw, "Writing nodes");
            }

            index = 0;
            File.AppendAllText(outputFile, "],\n");


            File.AppendAllText(outputFile, "\"links\":[");
            foreach (var edge in wordOutput.links)
            {
                index++;

                var str2 = "{\"source\":\"" + edge.source + "\",\"target\":\"" + edge.target + "\",\"value\":1}";

                if (index < wordOutput.links.Count)
                {
                    str2 += ",";
                }
                str2 += "\n";

                File.AppendAllText(outputFile, str2);
                Progress.OutputTimeRemaining(index, wordOutput.links.Count, sw, "Writing edges");
            }

            File.AppendAllText(outputFile, "]\n }");

        }



    }

    public class WordNode
    {
        public string id { get; set; }

        public string Group { get; set; }
    }



    public class WordEdge
    {
        public string source { get; set; }

        public string target { get; set; }
    }

    public class WordOutput
    {
        public HashSet<WordNode> nodes { get; set; }
        public HashSet<WordEdge> links { get; set; }
    }
}