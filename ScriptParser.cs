using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserCombinatorSample
{
    using static Parsers;

    public static class ScriptParser
    {
        public static Parser<IScriptNode> Parser => src =>
            _InnerParser.End()(src);

        private static Parser<IScriptNode> _TagParser => src =>
            _StartTagParser.Sequence(_InnerParser, (f, s) => (IScriptNode)new TagNode(f.tagName, f.attributes, (InnerNode)s))
                          .ValidateSequence(_EndTagParser, (f, s) => f, (f, s) => ((TagNode)f).TagName == s)
                          (src);

        private static Parser<IScriptNode> _InnerParser => src =>
            _TagParser.Or(_TextParser)
                     .Many()
                     .Map(x => (IScriptNode)new InnerNode(x))
                     (src);

        private static Parser<(string tagName, Dictionary<string, string> attributes)> _StartTagParser => src =>
            Just('<').None(_SpaceParser)
                     .Right(Letter.OneOrMore().Map())
                     .Sequence(_SpaceParser.Right(_AttributeParser).Many().Validate(x => x.GroupBy(xx => xx.Key).Count() == x.Count()),
                               (f, s) => (tagName: f, attributes: s.ToDictionary(ss => ss.Key, ss => ss.Value)))
                     .Left(_SpaceParser)
                     .Left(Just('>'))
                     (src);
        
        private static Parser<string> _EndTagParser => src =>
            Just('<').None(_SpaceParser)
                     .None(Just('/'))
                     .None(_SpaceParser)
                     .Right(Letter.OneOrMore().Map())
                     .Left(_SpaceParser)
                     .Left(Just('>'))
                     (src);

        private static Parser<KeyValuePair<string, string>> _AttributeParser => src =>
            Letter.OneOrMore().Map().Left(_SpaceParser)
                                    .Left(Just('='))
                                    .Left(_SpaceParser)
                                    .Left(Just('"'))
                                    .Sequence(Not('"').Many().Map(), (f, s) => new KeyValuePair<string, string>(f, s))
                                    .Left(Just('"'))
                                    (src);

        private static Parser<IScriptNode> _TextParser => src =>
            Not('<', '>').OneOrMore().Map().Map(x => (IScriptNode)new TextNode(x))(src);

        private static Parser<object> _SpaceParser => src =>
            Space.Many().None()(src);
    }
}
