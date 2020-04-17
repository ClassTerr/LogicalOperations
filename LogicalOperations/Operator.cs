using System;

namespace LogicalOperations
{
    /// <summary>
    ///     Class Operator, represents an Operator by holding information about it's symbol
    ///     the number of arguments it takes, the operator precedence and the delegate to use
    ///     for evaluating.
    /// </summary>
    [Serializable]
    public class Operator
    {
        /// <summary>
        ///     Delegate for evaluating this operator
        /// </summary>
        /// <param name="parser">reference to parser to use for evaluating arguments</param>
        /// <param name="arg1">first argument</param>
        /// <param name="arg2">second argument</param>
        /// <returns>result after operator evaluation</returns>
        public delegate double Evaluator(ExpressionParser parser, Node arg1, Node arg2);

        /// <summary>
        ///     Creates an Operator
        /// </summary>
        /// <param name="_operator">string operator symbol</param>
        /// <param name="arguments">number of arguments</param>
        /// <param name="precedence">precedence relative to other operators</param>
        /// <param name="eval">delegate to use for evaluating operator</param>
        public Operator(string _operator, int arguments, int precedence, Evaluator eval)
        {
            Symbol = _operator;
            Arguments = arguments;
            Precedence = precedence;
            Eval = eval;
        }

        /// <summary>
        ///     Provides access to the delegate for evaluating the operator
        /// </summary>
        public Evaluator Eval { get; }

        /// <summary>
        ///     Returns the precedence for this Operator.
        /// </summary>
        public int Precedence { get; } = Int32.MaxValue;

        /// <summary>
        ///     Returns the string symbol of this Operator.
        /// </summary>
        public string Symbol { get; } = "";

        /// <summary>
        ///     Returns the number of arguments this Operator can take.
        /// </summary>
        public int Arguments { get; }

        //кусь
        public override string ToString()
        {
            return Symbol;
        }
    } // End class Operator
}