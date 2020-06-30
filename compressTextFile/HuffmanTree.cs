using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using System.IO;

namespace HuffmanTest
{
    public class HuffmanTree
    {
        private static List<Node> nodes = new List<Node>();
        private static Node Root { get; set; }


        public static bool Encode(string inputFileName, string outputFileName)
        {

            if (!File.Exists(inputFileName))
            {
                return false;
            }

            string source = File.ReadAllText(inputFileName);
            
            Dictionary<char, int> Frequencies = BuildFrequencies(source);

            BuildTree(Frequencies);

            byte[] encodedData = dataEncode(source);

            using (FileStream binaryFile = File.Open(outputFileName, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(binaryFile))
                {
                    writer.Write(Frequencies.Count);
                    foreach (var kvp in Frequencies)
                    {
                        writer.Write(kvp.Key);
                        writer.Write(kvp.Value);
                    }
                    writer.Write(encodedData.Length - 1);
                    writer.Write(encodedData);
                }
            }

            return true;
        }


        public static bool Decode(string inputFilename, string outputFileName)
        {
            Dictionary<char, int> Frequencies = new Dictionary<char, int>();

            if (!File.Exists(inputFilename))
            {
                return false;
            }

            using (FileStream stream = new FileStream(inputFilename, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int count = reader.ReadInt32();

                    for (int n = 0; n < count; n++)
                    {
                        var key = reader.ReadChar();
                        var value = reader.ReadInt32();
                        Frequencies.Add(key, value);
                    }
                    BuildTree(Frequencies);
                    int dataSize = reader.ReadInt32();
                    byte[] allEncodedData = reader.ReadBytes(dataSize);

                    string decodedData = dataDecode(new BitArray(allEncodedData));

                    File.WriteAllText(outputFileName, decodedData);
                }
            }

            return true;
        }

        private static Dictionary<char, int> BuildFrequencies(string source)
        {
            Dictionary<char, int> Frequencies = new Dictionary<char, int>();

            for (int i = 0; i < source.Length; i++)
            {
                if (!Frequencies.ContainsKey(source[i]))
                {
                    Frequencies.Add(source[i], 0);
                }

                Frequencies[source[i]]++;
            }

            return Frequencies;
        }

        private static void BuildTree(Dictionary<char, int> Frequencies)
        {
            foreach (KeyValuePair<char, int> symbol in Frequencies)
            {
                nodes.Add(new Node() { Symbol = symbol.Key, Frequency = symbol.Value });
            }

            while (nodes.Count > 1)
            {
                List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList();

                if (orderedNodes.Count >= 2)
                {
                    // Take first two items
                    List<Node> taken = orderedNodes.Take(2).ToList();

                    // Create a parent node by combining the frequencies
                    Node parent = new Node()
                    {
                        Symbol = '*',
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }

                Root = nodes.FirstOrDefault();
            }
        }

        private static byte[] dataEncode(string source)
        {
            List<bool> encodedSource = new List<bool>();

            for (int i = 0; i < source.Length; i++)
            {
                List<bool> encodedSymbol = Root.Traverse(source[i], new List<bool>());
                encodedSource.AddRange(encodedSymbol);
            }

            BitArray bitArrayData = new BitArray(encodedSource.ToArray());

            // Convert to byte[] because its easier to to write it into a binary file
            byte[] encodedByte = new byte[bitArrayData.Length / 8 + 1];
            bitArrayData.CopyTo(encodedByte, 0);

            return encodedByte;
        }

        private static string dataDecode(BitArray bits)
        {
            Node current = Root;
            string decoded = "";

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (current.Right != null)
                    {
                        current = current.Right;
                    }
                }
                else
                {
                    if (current.Left != null)
                    {
                        current = current.Left;
                    }
                }

                if (IsLeaf(current))
                {
                    decoded += current.Symbol;
                    current = Root;
                }
            }

            return decoded;
        }

        public static bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right == null);
        }

    }
}