using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Conflicted.Grammar.Tokens
{
    sealed class IdentifierLexer : Lexer<IdentifierToken>
    {
        public override IdentifierToken Consume(Source source)
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

            while (source.CanPeek() && (char.IsLetter(source.Peek()) || char.IsDigit(source.Peek()) || source.Peek() == '_'))
            {
                text.Append(source.Read());
            }

            return new IdentifierToken(text.ToString());
        }

        public override bool IsMatch(Source source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.CanPeek())
            {
                return false;
            }

            return char.IsLetter(source.Peek());
        }
    }

    sealed class IdentifierToken : Token
    {
        public IdentifierToken(string text) : base(text)
        {
        }
    }
}
