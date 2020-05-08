using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Conflicted.Grammar.Tokens
{
    sealed class WhitespaceLexer : Lexer<WhitespaceToken>
    {
        public override WhitespaceToken Consume(Source source)
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

            while (source.CanPeek() && char.IsWhiteSpace(source.Peek()))
            {
                text.Append(source.Read());
            }

            return new WhitespaceToken(text.ToString());
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

            return char.IsWhiteSpace(source.Peek());
        }
    }

    sealed class WhitespaceToken : Token
    {
        public WhitespaceToken(string text) : base(text)
        {
        }
    }
}
