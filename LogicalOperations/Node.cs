using System;

namespace MathParserTestNS
{
    /// <summary>
    ///     Class Node, represents a Node in a tree data structure representation of a mathematical expression.
    /// </summary>
    [Serializable]
    public class Node
    {
        /// <summary>
        ///     Backing variable for property. Should absolutely not be serialized
        ///     since that will cause stale values to be persisted.
        /// </summary>
        [NonSerialized] private Value value;

        /// <summary>
        ///     Creates a Node containing the specified Operator and arguments.
        ///     This will automatically mark this Node as a TYPE_EXPRESSION
        /// </summary>
        /// <param name="_operator">the string representing an operator</param>
        /// <param name="_arg1">the first argument to the specified operator</param>
        /// <param name="_arg2">the second argument to the specified operator</param>
        public Node(Operator op, Node arg1, Node arg2)
        {
            FirstArgument = arg1;
            SecondArgument = arg2;
            Operator = op;
            Arguments = 2;

            Type = NodeType.Expression;
        }

        /// <summary>
        ///     Creates a Node containing the specified Operator and argument.
        ///     This will automatically mark this Node as a TYPE_EXPRESSION
        /// </summary>
        /// <param name="_operator">the string representing an operator</param>
        /// <param name="_arg1">the argument to the specified operator</param>
        public Node(Operator op, Node arg1)
        {
            FirstArgument = arg1;
            Operator = op;
            Arguments = 1;

            Type = NodeType.Expression;
        }

        /// <summary>
        ///     Creates a Node containing the specified variable.
        ///     This will automatically mark this Node as a TYPE_VARIABLE
        /// </summary>
        /// <param name="variable">the string representing a variable</param>
        public Node(string variable)
        {
            Variable = variable;
            Type = NodeType.Variable;
        }

        /// <summary>
        ///     Creates a Node containing the specified value.
        ///     This will automatically mark this Node as a TYPE_CONSTANT
        /// </summary>
        /// <param name="value">the value for this Node</param>
        public Node(double value)
        {
            this.value = new DoubleValue { Value = value };
            Type = NodeType.Value;
        }

        /// <summary>
        ///     Returns the String operator of this Node
        /// </summary>
        public Operator Operator { get; }

        /// <summary>
        ///     Gets or sets the value of this Node
        /// </summary>
        public Value Value
        {
            get => value;
            set => this.value = value;
        }

        /// <summary>
        ///     Returns the String variable of this Node
        /// </summary>
        public string Variable { get; }

        /// <summary>
        ///     Returns the number of arguments this Node has
        /// </summary>
        public int Arguments { get; }

        /// <summary>
        ///     Returns the node type
        /// </summary>
        public NodeType Type { get; }

        /// <summary>
        ///     Returns the first argument of this Node
        /// </summary>
        public Node FirstArgument { get; }

        /// <summary>
        ///     Returns the second argument of this Node
        /// </summary>
        public Node SecondArgument { get; }

        //кусь
        public string Label { get; set; }

        public string FullExpression { get; set; }

        public string ShortExpression { get; set; }
    } // End class Node
}