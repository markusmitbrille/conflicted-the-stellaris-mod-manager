using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Conflicted.Grammar.Tokens
{
    sealed class StringLexer : Lexer<StringToken>
    {
        public override StringToken Consume(Source source)
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

            while (source.CanPeek() && source.Peek() != '"')
            {
                text.Append(source.Read());
            }

            if (source.CanPeek() && source.Peek() == '"')
            {
                text.Append(source.Read());
            }

            return new StringToken(text.ToString());
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

            return source.Peek() == '"';
        }
    }

    sealed class StringToken : Token
    {
        public StringToken(string text) : base(text)
        {
        }
    }
}
