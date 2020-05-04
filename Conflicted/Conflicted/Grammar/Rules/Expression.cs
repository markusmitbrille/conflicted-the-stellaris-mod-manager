using System;
using System.Collections.Generic;
using System.Text;

namespace Conflicted.Grammar.Rules
{
    sealed class ExpressionParser : Parser<ExpressionRule>
    {
        public override ExpressionRule Consume(BookReader source)
        {
            throw new NotImplementedException();
        }

        public override bool IsMatch(BookReader source)
        {
            throw new NotImplementedException();
        }
    }

    sealed class ExpressionRule : Rule
    {
        public override IEnumerable<IAbstractSyntaxNode> Children => throw new NotImplementedException();

        public ExpressionRule(IAbstractSyntaxTree parent) : base(parent)
        {
        }
    }
}
