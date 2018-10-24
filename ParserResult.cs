
namespace ParserCombinatorSample
{
    public struct ParserResult<T>
    {
        public ParsedSrc Src { get; }
        public bool IsSuccess { get; }
        public T Result => IsSuccess ? _Result : throw new ParseException(Reason);
        public string Reason { get; }

        private T _Result { get; }

        public static ParserResult<T> Success(ParsedSrc src, T result)
        {
            return new ParserResult<T>(src, true, result, null);
        }

        public static ParserResult<T> Fail(ParsedSrc src, string reason)
        {
            return new ParserResult<T>(src, false, default(T), reason);
        }

        private ParserResult(ParsedSrc src, bool isSuccess, T result, string reason)
        {
            Src = src;
            IsSuccess = isSuccess;
            _Result = result;
            Reason = reason;
        }
    }
}
