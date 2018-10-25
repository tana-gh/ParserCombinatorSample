
namespace ParserCombinatorSample
{
    public struct ParserResult<T>
    {
        public ParsedSrc Src { get; }
        public bool IsSuccess { get; }
        public T Result => IsSuccess ? _Result : throw new ParseException(Reason);
        public string Reason { get; }
        public (int line, int letter) LinePos { get; set; }

        private T _Result { get; }

        public static ParserResult<T> Success(ParsedSrc src, T result)
        {
            return new ParserResult<T>(src, true, result, null, (0, 0));
        }

        public static ParserResult<T> Fail(ParsedSrc src, string reason, (int line, int letter) linePos)
        {
            return new ParserResult<T>(src, false, default(T), reason, linePos);
        }

        private ParserResult(ParsedSrc src, bool isSuccess, T result, string reason, (int line, int letter) linePos)
        {
            Src       = src;
            IsSuccess = isSuccess;
            _Result   = result;
            Reason    = reason;
            LinePos   = linePos;
        }
    }
}
