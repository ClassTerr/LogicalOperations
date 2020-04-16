using System;

namespace MathParserTestNS
{
    /// <summary>
    /// Exception class for parser related exceptions
    /// </summary>
    public class ParserException : System.Exception
    {
        public ParserException()
            : base()
        {

        }

        public ParserException(string message)
            : base(message)
        {

        }

        public ParserException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
