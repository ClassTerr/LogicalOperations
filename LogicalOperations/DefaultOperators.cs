using System.Collections.Generic;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace MathParserTestNS
{
    /// <summary>
    ///     Provides operators for evaluation
    /// </summary>
    public class DefaultOperators
    {
        public DefaultOperators()
        {
            Operators = new List<Operator>
            {
                new Operator("!", 1, 1, (p, a, b) => p.EvalTree(a) == 1 ? 0 : 1),
                new Operator("¬", 1, 1, (p, a, b) => p.EvalTree(a) == 1 ? 0 : 1),
                new Operator("⋀", 2, 2, (p, a, b) => p.EvalTree(a) == 1 && p.EvalTree(b) == 1 ? 1 : 0),
                new Operator("⋁", 2, 3, (p, a, b) => p.EvalTree(a) == 1 || p.EvalTree(b) == 1 ? 1 : 0),
                new Operator("V", 2, 3, (p, a, b) => p.EvalTree(a) == 1 && p.EvalTree(b) == 1 ? 1 : 0),
                new Operator("→", 2, 4, (p, a, b) => p.EvalTree(a) == 1 && p.EvalTree(b) == 0 ? 0 : 1),
                new Operator("↔", 2, 4, (p, a, b) => p.EvalTree(a) == p.EvalTree(b) ? 1 : 0),
                new Operator("~", 2, 4, (p, a, b) => p.EvalTree(a) == p.EvalTree(b) ? 1 : 0),
                new Operator("⊕", 2, 4, (p, a, b) => p.EvalTree(a) != p.EvalTree(b) ? 1 : 0),

                // Priority is not determined
                new Operator("|", 2, 4, (p, a, b) => p.EvalTree(a) == 1 && p.EvalTree(b) == 1 ? 0 : 1),
                new Operator("↓", 2, 4, (p, a, b) => p.EvalTree(a) == 1 || p.EvalTree(b) == 1 ? 0 : 1)
            };
        }

        /// <summary>
        ///     List of operators
        /// </summary>
        public IList<Operator> Operators { get; }
    }
}