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
                return ParserResult<char>.Fail(default(ParsedSrc), ex.Message);
            }
        };

        public static Parser<char> Letter { get; } = src =>
        {
            try
            {
                var (c, next) = src.Read();
                return char.IsLetterOrDigit(c) ? ParserResult<char>.Success(next, c) :
                                                 ParserResult<char>.Fail(src, $"{c} is not letter.");
            }
            catch (EndOfSrcException ex)
            {
                return ParserResult<char>.Fail(default(ParsedSrc), ex.Message);
            }
        };

        public static Parser<char> Space { get; } = src =>
        {
            try
            {
                var (c, next) = src.Read();
                return char.IsSeparator(c) ? ParserResult<char>.Success(next, c) :
                                             ParserResult<char>.Fail(src, $"{c} is not space.");
            }
            catch (EndOfSrcException ex)
            {
                return ParserResult<char>.Fail(default(ParsedSrc), ex.Message);
            }
        };

        public static Parser<char> Just(char literal) => src =>
        {
            try
            {
                var (c, next) = src.Read();
                return c == literal ? ParserResult<char>.Success(next, c) :
                                      ParserResult<char>.Fail(src, $"{c} does not equal {literal}.");
            }
            catch (EndOfSrcException ex)
            {
                return ParserResult<char>.Fail(default(ParsedSrc), ex.Message);
            }
        };

        public static Parser<char> Not(params char[] literals) => src =>
        {
            try
            {
                var (c, next) = src.Read();
                return !literals.Contains(c) ? ParserResult<char>.Success(next, c) :
                                               ParserResult<char>.Fail(src, $"{c} equals {string.Join(", ", literals)}.");
            }
            catch (EndOfSrcException ex)
            {
                return ParserResult<char>.Fail(default(ParsedSrc), ex.Message);
            }
        };

        public static Parser<char> Is(Func<char, bool> predicate) => src =>
        {
            try
            {
                var (c, next) = src.Read();
                return predicate(c) ? ParserResult<char>.Success(next, c) :
                                      ParserResult<char>.Fail(src, $"predicate({c}) failed.");
            }
            catch (EndOfSrcException ex)
            {
                return ParserResult<char>.Fail(default(ParsedSrc), ex.Message);
            }
        };
    }
}
