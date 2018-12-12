using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Day8
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string> linesReadingTask = System.IO.File.ReadAllTextAsync("..\\..\\..\\input.txt");

            string text = linesReadingTask.Result;

            var nodes = ParseInput(text);

            int metadataSum = nodes.Select(n => n.Metadata).SelectMany(m => m).Sum();

            // Task 1
            Console.WriteLine($"Sum of metadata: {metadataSum}");

            // Task 2
            Console.WriteLine($"Value of the root node: {nodes.First().Value}");

            Console.ReadKey();
        }

        public static List<Node> ParseInput(string text)
        {
            var nodes = new List<Node>();
            string[] partsS = text.Split(' ');
            List<int> parts = new List<int>();
            
            foreach (var partS in partsS)
            {
                parts.Add(int.Parse(partS));
            }

            // True if the algorithm is currently going deeper in the tree, looking for unvisited children
            // False if the algorithm is currently parsing the metadata of an element without unvisited children
            bool ongoingSubtreeReading = true;

            for (int i = 0; i < parts.Count; i++)
            {
                if (ongoingSubtreeReading)
                {
                    Node currentParentNode = nodes.LastOrDefault(n => n.ChildNodes.Count != n.ChildNumber);

                    var newNode = new Node(parts[i], parts[i + 1], currentParentNode);
                    newNode.ParsingFinished = (newNode.ChildNumber == 0 && newNode.MetadataNumber == 0);

                    if (currentParentNode != null)
                    {
                        currentParentNode.ChildNodes.Add(newNode);
                    }

                    nodes.Add(newNode);
                    
                    if (newNode.ChildNumber == 0)
                    {
                        ongoingSubtreeReading = false;
                    }
                    else
                    {
                        ongoingSubtreeReading = true;
                    }

                    // skip the new node's metadatanumber definition
                    i++;
                }
                else // if no subtree reading, the current data is the lastly added node's metadata entry
                {
                    var currentNode = nodes.Last(n => !n.ParsingFinished);

                    if (currentNode.Metadata.Count == currentNode.MetadataNumber - 1)
                    {
                        currentNode.Metadata.Add(parts[i]);
                        HandleFinishedMetadataParsing(currentNode, ref ongoingSubtreeReading);
                    }
                    else if (currentNode.Metadata.Count == currentNode.MetadataNumber)
                    {
                        HandleFinishedMetadataParsing(currentNode, ref ongoingSubtreeReading);
                    }
                    else
                    {
                        currentNode.Metadata.Add(parts[i]);
                    }
                }
            } // end for

            return nodes;
        }

        private static void HandleFinishedMetadataParsing(Node currentNode, ref bool ongoingSubtreeReading)
        {
            currentNode.ParsingFinished = true;
            if (currentNode.Parent != null &&
                currentNode.Parent.ChildNumber != currentNode.Parent.ChildNodes.Count)
            {
                ongoingSubtreeReading = true;
            }
            else
            {
                ongoingSubtreeReading = false;
            }
        }
    }

    [DebuggerDisplay("Child number: {ChildNumber}, Metadata number: {MetadataNumber}")]
    class Node
    {
        public Node(int childNumber, int metadataNumber, Node parent)
        {
            ChildNumber = childNumber;
            MetadataNumber = metadataNumber;
            Parent = parent;
            ChildNodes = new List<Node>();
            Metadata = new List<int>();
        }

        public Node Parent { get; }

        public int ChildNumber { get; }

        public int MetadataNumber { get; }
        
        public List<Node> ChildNodes { get; }

        public List<int> Metadata { get; }

        public bool ParsingFinished { get; set; }

        public int Value
        {
            get
            {
                if (ChildNumber == 0)
                    return Metadata.Sum();

                int value = 0;
                for (int i = 0; i < Metadata.Count; i++)
                {
                    // since index E [0, n], but metadata E [1, n]
                    int referencedChildIndex = Metadata[i] - 1;

                    // invalid reference
                    if (referencedChildIndex >= ChildNodes.Count)
                    {
                        continue;
                    }

                    Node referencedChild = ChildNodes[referencedChildIndex];
                    value += referencedChild.Value;
                }

                return value;
            }
        }
    }
}
