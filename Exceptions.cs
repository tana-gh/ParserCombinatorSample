using System;

namespace ParserCombinatorSample
{
    public class ParseException : Exception
    {
        internal ParseException(string reason)
            : base($"Parse failed: {reason}")
        {
        }
    }

    public class EndOfSrcException : Exception
    {
        public ParsedSrc Src { get; }

        internal EndOfSrcException(ParsedSrc src)
            : base("Reached end of source.")
        {
            Src = src;
        }
    }
}
