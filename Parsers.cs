using System;
using System.Linq;

namespace ParserCombinatorSample
{
    public delegate ParserResult<T> Parser<T>(ParsedSrc src);

    public static class Parsers
    {
        public static Parser<char> Any { get; } = src =>
        {
            try
            {
                var (c, next) = src.Read();
                return ParserResult<char>.Success(next, c);
            }
            catch (EndOfSrcException ex)
            {
                return ParserResult<char>.Fail(src, ex.Message, src.GetEndLinePos());
            }
        };

        public static Parser<char> Letter { get; } = src =>
        {
            try
            {
                var (c, next) = src.Read();
                return char.IsLetterOrDigit(c) ? ParserResult<char>.Success(next, c) :
                                                 ParserResult<char>.Fail(src, $"{c} is not letter.", next.GetLinePos());
            }
            catch (EndOfSrcException ex)
            {
                return ParserResult<char>.Fail(src, ex.Message, src.GetEndLinePos());
            }
        };

        public static Parser<char> Space { get; } = src =>
        {
            try
            {
                var (c, next) = src.Read();
                return c == ' ' || c == '\t' ? ParserResult<char>.Success(next, c) :
                                               ParserResult<char>.Fail(src, $"{c} is not space.", next.GetLinePos());
            }
            catch (EndOfSrcException ex)
            {
                return ParserResult<char>.Fail(src, ex.Message, src.GetEndLinePos());
            }
        };

        public static Parser<char> NewLine { get; } = src =>
        {
            try
            {
                var (c, next) = src.Read();
                return c == '\r' || c == '\n' ? ParserResult<char>.Success(next, c) :
                                                ParserResult<char>.Fail(src, $"{c} is not line separator.", next.GetLinePos());
            }
            catch (EndOfSrcException ex)
            {
                return ParserResult<char>.Fail(src, ex.Message, src.GetEndLinePos());
            }
        };

        public static Parser<char> Just(char literal) => src =>
        {
            try
            {
                var (c, next) = src.Read();
                return c == literal ? ParserResult<char>.Success(next, c) :
                                      ParserResult<char>.Fail(src, $"{c} does not equal {literal}.", next.GetLinePos());
            }
            catch (EndOfSrcException ex)
            {
                return ParserResult<char>.Fail(src, ex.Message, src.GetEndLinePos());
            }
        };

        public static Parser<char> Not(params char[] literals) => src =>
        {
            try
            {
                var (c, next) = src.Read();
                return !literals.Contains(c) ? ParserResult<char>.Success(next, c) :
                                               ParserResult<char>.Fail(src, $"{c} equals {string.Join(", ", literals)}.", next.GetLinePos());
            }
            catch (EndOfSrcException ex)
            {
                return ParserResult<char>.Fail(src, ex.Message, src.GetEndLinePos());
            }
        };

        public static Parser<char> Is(Func<char, bool> predicate) => src =>
        {
            try
            {
                var (c, next) = src.Read();
                return predicate(c) ? ParserResult<char>.Success(next, c) :
                                      ParserResult<char>.Fail(src, $"predicate({c}) failed.", next.GetLinePos());
            }
            catch (EndOfSrcException ex)
            {
                return ParserResult<char>.Fail(src, ex.Message, src.GetEndLinePos());
            }
        };
    }
}
