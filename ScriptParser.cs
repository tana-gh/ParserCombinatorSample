using System.Collections.Generic;
using System.Linq;

namespace ParserCombinatorSample
{
    using static Parsers;

    public static class ScriptParser
    {
        public static Parser<IScriptNode> Parser => src =>
            _SpaceOrNewLineParser.Right(_TagParser).Left(_SpaceOrNewLineParser).End()(src);

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
            Just('<').None(_SpaceOrNewLineParser)
                     .Right(Letter.OneOrMore().Map())
                     .Sequence(_SpaceOrNewLineParser.Right(_AttributeParser).Many().Validate(x => x.GroupBy(xx => xx.Key).Count() == x.Count()),
                               (f, s) => (tagName: f, attributes: s.ToDictionary(ss => ss.Key, ss => ss.Value)))
                     .Left(_SpaceOrNewLineParser)
                     .Left(Just('>'))
                     (src);
        
        private static Parser<string> _EndTagParser => src =>
            Just('<').None(_SpaceOrNewLineParser)
                     .None(Just('/'))
                     .None(_SpaceOrNewLineParser)
                     .Right(Letter.OneOrMore().Map())
                     .Left(_SpaceOrNewLineParser)
                     .Left(Just('>'))
                     (src);

        private static Parser<KeyValuePair<string, string>> _AttributeParser => src =>
            Letter.OneOrMore().Map().Left(_SpaceOrNewLineParser)
                                    .Left(Just('='))
                                    .Left(_SpaceOrNewLineParser)
                                    .Left(Just('"'))
                                    .Sequence(Not('"').Many().Map(), (f, s) => new KeyValuePair<string, string>(f, s))
                                    .Left(Just('"'))
                                    (src);

        private static Parser<IScriptNode> _TextParser => src =>
            _SpaceOrNewLineParser.Right(Not('<', '>', '\r', '\n').OneOrMore())
                                 .Left(NewLine.Many())
                                 .Map().Map(x => (IScriptNode)new TextNode(x))(src);

        private static Parser<object> _SpaceOrNewLineParser => src =>
            Space.Or(NewLine).Many().None()(src);
    }
}
