﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LogicalOperations
{
    public class TreeParser
    {
        private readonly IDictionary<string, double> constants;
        private CultureInfo culture;

        private readonly int maxOpLength;
        private readonly IDictionary<string, Operator> operators;

        /// <summary>
        ///     Constructs a TreeParser instance
        /// </summary>
        /// <param name="operators">operators to use when creating tree representation of an expression</param>
        /// <param name="constants">constants to use when creating tree representation of an expression</param>
        public TreeParser(IDictionary<string, Operator> operators, IDictionary<string, double> constants)
        {
            this.operators = operators;
            this.constants = constants;

            foreach (var pair in operators)
            {
                var symbol = pair.Value.Symbol;

                if (symbol.Length > maxOpLength)
                {
                    maxOpLength = symbol.Length;
                }
            }

            culture = CultureInfo.InvariantCulture;
        }

        /// <summary>
        ///     Enables or disables the requirement to put all function arguments within parantheses.
        ///     Default is true making it required for all function arguments to be enclosed within () for example sin(x+5)
        ///     failure to do so will generate an exception stating that parantheses are required.
        ///     Setting this property to false disables this requirement making expressions such as for example sinx or sin5
        ///     allowed.
        /// </summary>
        public bool RequireParentheses { get; set; }

        /// <summary>
        ///     Enables or disables the support for implicit multiplication.
        ///     Default is true making implicit multiplication allowed, for example 2x or 3sin(2x)
        ///     setting this property to false disables support for implicit multiplication making the * operator required
        ///     for example 2*x or 3*sin(2*x)
        ///     If implicit multiplication is disabled (false) parsing an expression that does not explicitly use the * operator
        ///     may
        ///     throw syntax errors with various error messages.
        /// </summary>
        public bool ImplicitMultiplication { get; set; }

        /// <summary>
        ///     Gets or sets the culture to use
        /// </summary>
        public CultureInfo Culture
        {
            get => culture;

            set
            {
                if (value == null)
                    throw new ArgumentNullException("Culture cannot be null");

                // Cannot allow division operator as decimal separator (fa, fa-IR)
                if (value.NumberFormat.NumberDecimalSeparator == "/")
                {
                    throw new ArgumentOutOfRangeException(String.Format("Unsupported decimal separator / culture {0}",
                        value.Name));
                }

                // Cannot allow same separators
                if (value.NumberFormat.CurrencyGroupSeparator == value.NumberFormat.NumberDecimalSeparator)
                {
                    throw new ArgumentOutOfRangeException(
                        String.Format("Same decimal and group separator is unsupported. Culture {0}", value.Name));
                }

                culture = value;
            }
        }

        /// <summary>
        ///     Checks the String expression to see if the syntax is valid.
        ///     this method doesn't return anything, instead it throws an Exception
        ///     if the syntax is invalid.
        ///     Examples of invalid syntax can be non matching paranthesis, non valid symbols appearing
        ///     or a variable or operator name is invalid in the expression.
        /// </summary>
        /// <param name="exp">the string expression to check, infix notation.</param>
        /// <remarks>
        ///     This validates some syntax errors such as unbalanced paranthesis and invalid characters.
        ///     Exceptions can also be thrown at evaluation time.
        /// </remarks>
        private void SyntaxCheck(string exp)
        {
            int i = 0, oplen = 0;
            string op = null;
            string nop = null;

            // Check if all paranthesis match in expression
            if (!matchParant(exp))
            {
                throw new ParserException("Unbalanced parenthesis");
            }

            var l = exp.Length;

            // Go through expression and validate syntax
            while (i < l)
            {
                try
                {
                    if ((op = getOp(exp, i)) != null)
                    {
                        // Found operator at position, check syntax for operators

                        oplen = op.Length;
                        i += oplen;

                        // If it's a function and we are missing parentheses around arguments and bRequireParantheses is true it is an error.
                        // Note that this only checks opening paranthesis, we checked whole expression for balanced paranthesis
                        // earlier but not for each individual function.
                        if (RequireParentheses && !isTwoArgOp(op) && op != "!" && op != "¬" && op != "￢" && exp[i] != '(')
                        {
                            throw new ParserException("Parenthesis required for arguments -> " +
                                                      exp.Substring(i - oplen));
                        }

                        // If we have an operator immediately following a function and it's not unary + or - then it's an error
                        nop = getOp(exp, i);

                        if (nop != null && isTwoArgOp(nop) && !(nop.Equals("+") || nop.Equals("-")))
                        {
                            throw new ParserException("Syntax error near -> " + exp.Substring(i - oplen));
                        }
                    }
                    else if (!isAlpha(exp[i]) && !isConstant(exp[i]) && !isAllowedSym(exp[i]))
                    {
                        // This cannot be a valid character, throw exception
                        throw new ParserException("Syntax error near -> " + exp.Substring(i));
                    }
                    else
                    {
                        // Count forward
                        i++;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    // Might happen when we are checking syntax, just move forward
                    i++;
                }
            }
        }

        /// <summary>
        ///     Parses an infix String expression and creates a parse tree of Node's.
        /// </summary>
        /// <remarks>
        ///     This is the heart of the parser, it takes a normal infix expression and creates
        ///     a tree datastructure we can easily recurse when evaluating.
        /// </remarks>
        /// <param name="exp">the infix string expression to process</param>
        /// <returns>A tree datastructure of Node objects representing the expression</returns>
        private Node ParseInfix(string exp)
        {
            int i, ma, len;
            string farg, sarg, fop;
            Node tree = null;

            farg = sarg = fop = "";
            ma = i = 0;

            len = exp.Length;

            if (len == 0)
            {
                throw new ParserException("Wrong number of arguments to operator");
            }

            if (exp[0] == '(' && (ma = match(exp, 0)) == len - 1)
            {
                return ParseInfix(exp.Substring(1, ma - 1));
            }

            if (isVariable(exp))
            {
                // If built in constant put in value otherwise the variable
                if (constants.ContainsKey(exp))
                    return new Node(constants[exp]);

                return new Node(exp);
            }

            if (isConstant(exp))
            {
                try
                {
                    return new Node(Double.Parse(exp, NumberStyles.Any, culture));
                }
                catch (FormatException)
                {
                    throw new ParserException("Syntax error-> " + exp + " (not using regional decimal separator?)");
                }
            }

            while (i < len)
            {
                if ((fop = getOp(exp, i)) == null)
                {
                    farg = arg(null, exp, i);
                    fop = getOp(exp, i + farg.Length);

                    if (fop == null)
                        throw new Exception("Missing operator");

                    if (isTwoArgOp(fop))
                    {
                        sarg = arg(fop, exp, i + farg.Length + fop.Length);

                        if (sarg.Equals(""))
                            throw new Exception("Wrong number of arguments to operator " + fop);

                        tree = new Node(operators[fop], ParseInfix(farg), ParseInfix(sarg));
                        i += farg.Length + fop.Length + sarg.Length;
                    }
                    else
                    {
                        if (farg.Equals(""))
                            throw new Exception("Wrong number of arguments to operator " + fop);

                        tree = new Node(operators[fop], ParseInfix(farg));
                        i += farg.Length + fop.Length;
                    }
                }
                else
                {
                    if (isTwoArgOp(fop))
                    {
                        farg = arg(fop, exp, i + fop.Length);

                        if (farg.Equals(""))
                            throw new Exception("Wrong number of arguments to operator " + fop);

                        if (tree == null)
                        {
                            if (fop.Equals("+") || fop.Equals("-"))
                            {
                                tree = new Node(0D);
                            }
                            else
                                throw new Exception("Wrong number of arguments to operator " + fop);
                        }

                        tree = new Node(operators[fop], tree, ParseInfix(farg));
                        i += farg.Length + fop.Length;
                    }
                    else
                    {
                        farg = arg(fop, exp, i + fop.Length);

                        if (farg.Equals(""))
                            throw new Exception("Wrong number of arguments to operator " + fop);

                        tree = new Node(operators[fop], ParseInfix(farg));
                        i += farg.Length + fop.Length;
                    }
                }
            }

            return tree;
        }

        public Expression Parse(string expression)
        {
            var tmp = skipSpaces(expression.ToLower());

            SyntaxCheck(tmp);

            var tree = ParseInfix(tmp);

            return new Expression(tree);
        }

        /// <summary>Matches all paranthesis and returns true if they all match or false if they do not.</summary>
        /// <param name="exp">expression to check, infix notation</param>
        /// <returns>true if ok false otherwise</returns>
        private bool matchParant(string exp)
        {
            var count = 0;
            var i = 0;

            var l = exp.Length;

            for (i = 0; i < l; i++)
            {
                if (exp[i] == '(')
                {
                    count++;
                }
                else if (exp[i] == ')')
                {
                    count--;
                }
            }

            return count == 0;
        }

        /// <summary>Checks if the character is alphabetic.</summary>
        /// <param name="ch">Character to check</param>
        /// <returns>true or false</returns>
        private bool isAlpha(char ch)
        {
            return ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z';
        }

        /// <summary>Checks if the string can be considered to be a valid variable name.</summary>
        /// <param name="str">The String to check</param>
        /// <returns>true or false</returns>
        private bool isVariable(string str)
        {
            var i = 0;
            var len = str.Length;

            if (isConstant(str))
                return false;

            for (i = 0; i < len; i++)
            {
                if (getOp(str, i) != null || isAllowedSym(str[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Checks if the character is a digit</summary>
        /// <param name="ch">Character to check</param>
        /// <returns>true or false</returns>
        private bool isConstant(char ch)
        {
            return Char.IsDigit(ch);
        }

        /// <summary>Checks to se if a string is numeric</summary>
        /// <param name="exp">String to check</param>
        /// <returns>true if the string was numeric, false otherwise</returns>
        private bool isConstant(string exp)
        {
            var val = 0D;
            var ok = Double.TryParse(exp, NumberStyles.Any, culture, out val);
            return ok && !Double.IsNaN(val);
        }

        /// <summary>
        ///     Checks to see if the string is the name of a acceptable operator.
        /// </summary>
        /// <param name="str">The string to check</param>
        /// <returns>true if it is an acceptable operator, false otherwise.</returns>
        private bool isOperator(string str)
        {
            return operators.ContainsKey(str);
        }

        /// <summary>
        ///     Checks to see if the operator name represented by str takes two arguments.
        /// </summary>
        /// <param name="str">The string to check</param>
        /// <returns>true if the operator takes two arguments, false otherwise.</returns>
        private bool isTwoArgOp(string str)
        {
            if (str == null)
                return false;

            object o = operators[str];

            if (o == null)
                return false;

            return ((Operator) o).Arguments == 2;
        }

        /// <summary>
        ///     Checks to see if the character is a valid symbol for this parser.
        /// </summary>
        /// <param name="s">the character to check</param>
        /// <returns>true if the char is valid, false otherwise.</returns>
        private bool isAllowedSym(char s)
        {
            return s == ',' || s == '.' || s == ')' || s == '(' || s == '>' || s == '<' || s == '&' || s == '=' ||
                   s == '|' || culture.NumberFormat.CurrencyGroupSeparator.ToCharArray().Contains(s) ||
                   culture.NumberFormat.CurrencyDecimalSeparator.ToCharArray().Contains(s);
        }

        /// <summary>
        ///     Parses out spaces from a string
        /// </summary>
        /// <param name="str">The string to process</param>
        /// <returns>A copy of the string stripped of all spaces</returns>
        private string skipSpaces(string str)
        {
            var i = 0;
            var len = str.Length;
            var nstr = new StringBuilder(len);

            while (i < len)
            {
                if (!Char.IsWhiteSpace(str[i]))
                {
                    nstr.Append(str[i]);
                }

                i++;
            }

            return nstr.ToString();
        }

        /// <summary>
        ///     Matches an opening left paranthesis.
        /// </summary>
        /// <param name="exp">the string to search in</param>
        /// <param name="index">the index of the opening left paranthesis</param>
        /// <returns>the index of the matching closing right paranthesis</returns>
        private int match(string exp, int index)
        {
            var len = exp.Length;
            var i = index;
            var count = 0;

            while (i < len)
            {
                if (exp[i] == '(')
                {
                    count++;
                }
                else if (exp[i] == ')')
                {
                    count--;
                }

                if (count == 0)
                    return i;

                i++;
            }

            return index;
        }

        /// <summary>
        ///     Parses out an operator from an infix string expression.
        /// </summary>
        /// <param name="exp">the infix string expression to look in</param>
        /// <param name="index">the index to start searching from</param>
        /// <returns>the operator if any or null.</returns>
        private string getOp(string exp, int index)
        {
            string tmp;
            var i = 0;
            var len = exp.Length;

            for (i = 0; i < maxOpLength; i++)
            {
                if (index >= 0 && index + maxOpLength - i <= len)
                {
                    tmp = exp.Substring(index, maxOpLength - i);

                    if (isOperator(tmp))
                    {
                        return tmp;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Parses the infix expression for arguments to the specified operator.
        /// </summary>
        /// <param name="_operator">the operator we are interested in</param>
        /// <param name="exp">the infix string expression</param>
        /// <param name="index">the index to start the search from</param>
        /// <returns>the argument to the operator</returns>
        private string arg(string _operator, string exp, int index)
        {
            int ma, i, prec = -1;
            var len = exp.Length;
            string op = null;

            var str = new StringBuilder();

            i = index;
            ma = 0;

            if (_operator == null)
            {
                prec = -1;
            }
            else
            {
                prec = operators[_operator].Precedence;
            }

            while (i < len)
            {
                if (exp[i] == '(')
                {
                    ma = match(exp, i);
                    str.Append(exp.Substring(i, ma + 1 - i));
                    i = ma + 1;
                }
                else if ((op = getOp(exp, i)) != null)
                {
                    // (_operator != null && _operator.Equals("&&") && op.Equals("||") ) || 
                    if (str.Length != 0 && !isTwoArgOp(backTrack(str.ToString())) && operators[op].Precedence >= prec)
                    {
                        return str.ToString();
                    }

                    str.Append(op);
                    i += op.Length;
                }
                else
                {
                    str.Append(exp[i]);
                    i++;
                }
            }

            return str.ToString();
        }

        /// <summary>
        ///     Returns an operator at the end of the String str if present.
        /// </summary>
        /// <remarks>
        ///     Used when parsing for arguments, the purpose is to recognize
        ///     expressions like for example 10^-1
        /// </remarks>
        /// <param name="str">part of infix string expression to search</param>
        /// <returns>the operator if found or null otherwise</returns>
        private string backTrack(string str)
        {
            var i = 0;
            var len = str.Length;
            string op = null;

            try
            {
                for (i = 0; i <= maxOpLength; i++)
                {
                    if ((op = getOp(str, len - 1 - maxOpLength + i)) != null &&
                        len - maxOpLength - 1 + i + op.Length == len)
                    {
                        return op;
                    }
                }
            }
            catch { }

            return null;
        }
    } // End class TreeParser
} // End namespace info.lundin.math