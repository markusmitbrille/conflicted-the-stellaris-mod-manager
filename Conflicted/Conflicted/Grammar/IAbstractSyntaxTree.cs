using System.Collections.Generic;

namespace Conflicted.Grammar
{
    interface IAbstractSyntaxTree : IAbstractSyntaxNode
    {
        public IAbstractSyntaxTree Parent { get; }
        public IEnumerable<IAbstractSyntaxNode> Children { get; }
    }
}
