using System;
using System.Text;

namespace ParserCombinatorSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new StringBuilder();
            
            for (;;)
            {
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    break;
                }

                builder.AppendLine(input);
            }

            var result = ScriptParser.Parser(new ParsedSrc(builder.ToString()));

            if (result.IsSuccess)
            {
                Console.WriteLine("OK");
            }
            else
            {
                var linePos = result.LinePos;
                Console.WriteLine($"Failed on ({linePos.line}, {linePos.letter}): {result.Reason}");
            }
        }
    }
}
