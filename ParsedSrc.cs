using System.Linq;

namespace ParserCombinatorSample
{
    public struct ParsedSrc
    {
        private string _Src { get; }
        private int _Pos { get; }
        
        public ParsedSrc(string src)
            : this(src, 0)
        {
        }

        private ParsedSrc(string src, int pos)
        {
            _Src = src;
            _Pos = pos;
        }

        public (char c, ParsedSrc src) Read()
        {
            if (_Pos + 1 > _Src.Length)
            {
                throw new EndOfSrcException(this);
            }

            return (_Src[_Pos], new ParsedSrc(_Src, _Pos + 1));
        }

        public (string s, ParsedSrc src) Read(int length)
        {
            if (_Pos + length > _Src.Length)
            {
                throw new EndOfSrcException(this);
            }

            return (_Src.Substring(_Pos, length), new ParsedSrc(_Src, _Pos + length));
        }

        public (int line, int letter) GetLinePos()
        {
            int line   = 0;
            int letter = 0;

            foreach (var c in _Src.Take(_Pos))
            {
                if (c == '\n')
                {
                    line++;
                    letter = 0;
                }
                else
                {
                    letter++;
                }
            }

            return (line, letter);
        }

        public bool isEnd()
        {
            return _Pos >= _Src.Length;
        }
    }
}
