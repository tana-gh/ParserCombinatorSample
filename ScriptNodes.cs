using System.Collections.Generic;

namespace ParserCombinatorSample
{
    public interface IScriptNode
    {
    }

    public class TagNode : IScriptNode
    {
        public string TagName { get; }
        public IReadOnlyDictionary<string, string> Attributes { get; }
        public InnerNode Inner { get; }

        internal TagNode(string tagName, IReadOnlyDictionary<string, string> attributes, InnerNode inner)
        {
            TagName    = tagName;
            Attributes = attributes;
            Inner      = inner;
        }
    }

    public class InnerNode : IScriptNode
    {
        public IEnumerable<IScriptNode> Nodes { get; }

        internal InnerNode(IEnumerable<IScriptNode> nodes)
        {
            Nodes = nodes;
        }
    }

    public class TextNode : IScriptNode
    {
        public string Text { get; }

        internal TextNode(string text)
        {
            Text = text;
        }
    }
}
