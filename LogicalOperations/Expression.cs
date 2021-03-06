﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LogicalOperations
{
    /// <summary>
    ///     Encapsulates the parsed expression tree and
    ///     provides methods for serialization
    /// </summary>
    public class Expression
    {
        /// <summary>
        ///     Creates instance
        /// </summary>
        public Expression() { }

        /// <summary>
        ///     Creates instance using the Node as the expression tree
        /// </summary>
        /// <param name="tree">expression tree of Node instances</param>
        public Expression(Node tree)
        {
            this.ExpressionTree = tree;
        }

        /// <summary>
        ///     Provides access to the expression tree
        /// </summary>
        public Node ExpressionTree { get; set; }

        /// <summary>
        ///     Serializes the expression to the stream
        /// </summary>
        /// <param name="stream">stream to write to</param>
        public void Save(Stream stream)
        {
            var bin = new BinaryFormatter();
            bin.Serialize(stream, ExpressionTree);
        }

        /// <summary>
        ///     Attempts to load a serialized expression from the stream
        /// </summary>
        /// <param name="stream">stream to read from</param>
        public void Load(Stream stream)
        {
            var bin = new BinaryFormatter();
            var tree = bin.Deserialize(stream) as Node;
            if (tree != null) ExpressionTree = tree;
        }
    }
}