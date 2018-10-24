using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserCombinatorSample
{
    public static class Combinators
    {
        public static Parser<T> End<T>(this Parser<T> parser) => src =>
        {
            var result = parser(src);

            return result.IsSuccess   ?
                   result.Src.isEnd() ? result :
                                        ParserResult<T>.Fail(src, "Cannot reach to end.") :
                                        ParserResult<T>.Fail(src, result.Reason);
        };

        public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser) => src =>
        {
            ParserResult<IEnumerable<T>> getResults(ParsedSrc s, IEnumerable<T> results)
            {
                var result = parser(s);

                if (result.IsSuccess)
                {
                    ((List<T>)results).Add(result.Result);
                    return getResults(result.Src, results);
                }
                else
                {
                    return ParserResult<IEnumerable<T>>.Success(s, results);
                }
            }

            return getResults(src, new List<T>());
        };

        public static Parser<IEnumerable<T>> OneOrMore<T>(this Parser<T> parser) => src =>
        {
            ParserResult<IEnumerable<T>> getResults(ParsedSrc s, IEnumerable<T> results)
            {
                var result = parser(s);

                if (result.IsSuccess)
                {
                    ((List<T>)results).Add(result.Result);
                    return getResults(result.Src, results);
                }
                else
                {
                    return results.Any() ? ParserResult<IEnumerable<T>>.Success(s, results) :
                                           ParserResult<IEnumerable<T>>.Fail(src, "Iterate count not fully.");
                }
            }

            return getResults(src, new List<T>());
        };

        public static Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int count) => src =>
        {
            ParserResult<IEnumerable<T>> getResults(ParsedSrc s, IEnumerable<T> results, int c)
            {
                if (c == 0)
                {
                    return ParserResult<IEnumerable<T>>.Success(s, results);
                }

                var result = parser(s);

                if (result.IsSuccess)
                {
                    ((List<T>)results).Add(result.Result);
                    return getResults(result.Src, results, c - 1);
                }
                else
                {
                    return ParserResult<IEnumerable<T>>.Fail(src, result.Reason);
                }
            }

            return getResults(src, new List<T>(), count);
        };

        public static Parser<IEnumerable<T>> Sequence<T>(this Parser<T> first, Parser<T> second) =>
            first.Sequence(second, (f, s) => (IEnumerable<T>)new List<T>() { f, s });

        public static Parser<IEnumerable<T>> Sequence<T>(this Parser<IEnumerable<T>> first, Parser<T> second) =>
            first.Sequence(second, (f, s) => { var list = new List<T>(); list.AddRange(f); list.Add(s); return (IEnumerable<T>)list; });

        public static Parser<TResult> Sequence<TFirst, TSecond, TResult>
        (
            this Parser<TFirst> first,
            Parser<TSecond>     second,
            Func<TFirst, TSecond, TResult> resultSelector
        )
        => ValidateSequence(first, second, resultSelector, (f, s) => true);

        public static Parser<T> Or<T>(this Parser<T> first, Parser<T> second) => src =>
        {
            var firstResult = first(src);

            return firstResult.IsSuccess ? firstResult : second(src);
        };

        public static Parser<TLeft> Left<TLeft, TRight>(this Parser<TLeft> left, Parser<TRight> right) => left.Sequence(right, (l, r) => l);

        public static Parser<TRight> Right<TLeft, TRight>(this Parser<TLeft> left, Parser<TRight> right) => left.Sequence(right, (l, r) => r);

        public static Parser<object> None<TLeft, TRight>(this Parser<TLeft> left, Parser<TRight> right) => left.Sequence(right, (l, r) => (object)null);

        public static Parser<object> None<T>(this Parser<T> parser) => parser.Map(x => (object)null);

        public static Parser<TResult> Map<TParser, TResult>(this Parser<TParser> parser, Func<TParser, TResult> resultSelector) => src =>
        {
            var result = parser(src);

            return result.IsSuccess ? ParserResult<TResult>.Success(result.Src, resultSelector(result.Result)) :
                                      ParserResult<TResult>.Fail(src, result.Reason);
        };

        public static Parser<string> Map(this Parser<char> parser) => src =>
        {
            var result = parser(src);

            return result.IsSuccess ? ParserResult<string>.Success(result.Src, result.Result.ToString()) :
                                      ParserResult<string>.Fail(src, result.Reason);
        };

        public static Parser<string> Map(this Parser<IEnumerable<char>> parser) => src =>
        {
            var result = parser(src);

            return result.IsSuccess ? ParserResult<string>.Success(result.Src, new string(result.Result.ToArray())) :
                                      ParserResult<string>.Fail(src, result.Reason);
        };

        public static Parser<IEnumerable<char>> Map(this Parser<string> parser) => src =>
        {
            var result = parser(src);

            return result.IsSuccess ? ParserResult<IEnumerable<char>>.Success(result.Src, result.Result) :
                                      ParserResult<IEnumerable<char>>.Fail(src, result.Reason);
        };

        public static Parser<T> Validate<T>(this Parser<T> parser, Func<T, bool> predicate) => src =>
        {
            var result = parser(src);

            return result.IsSuccess         ?
                   predicate(result.Result) ? result :
                                              ParserResult<T>.Fail(src, "Validation failed.") :
                                              ParserResult<T>.Fail(src, result.Reason);
        };

        public static Parser<TResult> ValidateSequence<TFirst, TSecond, TResult>
        (
            this Parser<TFirst> first,
            Parser<TSecond>     second,
            Func<TFirst, TSecond, TResult> resultSelector,
            Func<TFirst, TSecond, bool>    predicate
        )
        => src =>
        {
            var firstResult = first(src);

            if (firstResult.IsSuccess)
            {
                var secondResult = second(firstResult.Src);

                return secondResult.IsSuccess                             ? 
                       predicate(firstResult.Result, secondResult.Result) ? ParserResult<TResult>.Success(secondResult.Src, resultSelector(firstResult.Result, secondResult.Result)) :
                                                                            ParserResult<TResult>.Fail(src, "Validation failed.") :
                                                                            ParserResult<TResult>.Fail(src, secondResult.Reason);
            }
            else
            {
                return ParserResult<TResult>.Fail(src, firstResult.Reason);
            }
        };
    }
}
