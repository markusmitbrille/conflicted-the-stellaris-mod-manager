using System.Collections.Generic;
using System.Linq;

namespace Conflicted.Grammar
{
    abstract class Rule : IAbstractSyntaxTree
    {
        private string text;
        public string Text => text ?? (text = string.Join(null, Children.Select(node => node.Text)));

        public bool IsRoot => Parent == null;
        public bool IsInternal => true;
        public bool IsLeaf => false;

        public IAbstractSyntaxTree Parent { get; }
        public abstract IEnumerable<IAbstractSyntaxNode> Children { get; }

        public Rule(IAbstractSyntaxTree parent)
        {
            Parent = parent;
        }
    }
}
