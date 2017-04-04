﻿using System.Collections.Generic;
using System.Linq;

namespace CodingTheory
{
    public class HuffmanCoding : IAlgorithm
    {
        private readonly List<Node> _nodes = new List<Node>();
        private readonly Dictionary<char, int> _frequencies = new Dictionary<char, int>();
        private double _compressionRatio = -1;

        public Node Root { get; set; }

        public string Encode(string input)
        {
            Build(input);

            List<bool> encodedSource = new List<bool>();

            for (int i = 0; i < input.Length; i++)
            {
                List<bool> encodedSymbol = this.Root.Traverse(input[i], new List<bool>());
                encodedSource.AddRange(encodedSymbol);
            }

            _compressionRatio = (double)encodedSource.Count / (input.Length * 8);

            return encodedSource.Aggregate("", (current, i) => current + (i ? "1" : "0"));
        }

        public string Decode(string input)
        {
            List<bool> bits = input.Select(i => i == '1').ToList();

            Node current = this.Root;
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

                if (!current.IsLeaf) continue;
                decoded += current.Symbol;
                current = this.Root;
            }

            return decoded;
        }

        public double GetCompressionRatio()
        {
            return _compressionRatio;
        }

        private void Build(string source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (!_frequencies.ContainsKey(source[i]))
                {
                    _frequencies.Add(source[i], 0);
                }

                _frequencies[source[i]]++;
            }

            foreach (KeyValuePair<char, int> symbol in _frequencies)
            {
                _nodes.Add(new Node() { Symbol = symbol.Key, Frequency = symbol.Value });
            }

            while (_nodes.Count > 1)
            {
                List<Node> orderedNodes = _nodes.OrderBy(node => node.Frequency).ToList();

                if (orderedNodes.Count >= 2)
                {
                    List<Node> taken = orderedNodes.Take(2).ToList();

                    Node parent = new Node()
                    {
                        Symbol = '*',
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    _nodes.Remove(taken[0]);
                    _nodes.Remove(taken[1]);
                    _nodes.Add(parent);
                }

                this.Root = _nodes.FirstOrDefault();

            }
        }

        public class Node
        {
            public char Symbol { get; set; }
            public int Frequency { get; set; }
            public Node Right { get; set; }
            public Node Left { get; set; }
            public bool IsLeaf
            {
                get
                {
                    return (Left == null && Right == null);
                }
                
            }

            public List<bool> Traverse(char symbol, List<bool> data)
            {
                if (Right == null && Left == null)
                {
                    return symbol.Equals(this.Symbol) ? data : null;
                }

                List<bool> left = null;
                List<bool> right = null;

                if (this.Left != null)
                {
                    List<bool> leftPath = new List<bool>();
                    leftPath.AddRange(data);
                    leftPath.Add(false);

                    left = this.Left.Traverse(symbol, leftPath);
                }

                if (this.Right != null)
                {
                    List<bool> rightPath = new List<bool>();
                    rightPath.AddRange(data);
                    rightPath.Add(true);

                    right = this.Right.Traverse(symbol, rightPath);
                }

                return left ?? right;
            }
        }
    }
}