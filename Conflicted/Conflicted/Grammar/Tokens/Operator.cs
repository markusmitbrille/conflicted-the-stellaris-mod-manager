using System;
using System.Collections.Generic;
using System.Text;

namespace Conflicted.Grammar.Tokens
{
    sealed class OperatorLexer : Lexer<OperatorToken>
    {
        public override OperatorToken Consume(SourceReader source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!IsMatch(source))
            {
                return null;
            }

            StringBuilder text = new StringBuilder();
            text.Append(source.Read());

            if (source.CanPeek() && source.Peek() == '=' && (source.Peek(-1) == '<' || source.Peek(-1) == '>'))
            {
                text.Append(source.Read());
            }

            return new OperatorToken(text.ToString());
        }

        public override bool IsMatch(SourceReader source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.CanPeek())
            {
                return false;
            }

            char next = source.Peek();

            if (next == '=') return true;
            if (next == '>') return true;
            if (next == '<') return true;

            return false;
        }
    }

    sealed class OperatorToken : Token
    {
        public OperatorToken(string text) : base(text)
        {
        }
    }
}
