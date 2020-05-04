using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Conflicted.Grammar.Tokens
{
    sealed class NumberLexer : Lexer<NumberToken>
    {
        public override NumberToken Consume(SourceReader source)
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

            while (source.CanPeek() && char.IsDigit(source.Peek()))
            {
                text.Append(source.Read());
            }

            return new NumberToken(text.ToString());
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

            if (char.IsDigit(source.Peek()))
            {
                return true;
            }

            if (source.Peek() == '+' && source.CanPeek(1) && char.IsDigit(source.Peek()))
            {
                return true;
            }
            if (source.Peek() == '-' && source.CanPeek(1) && char.IsDigit(source.Peek()))
            {
                return true;
            }

            return false;
        }
    }

    sealed class NumberToken : Token
    {
        public NumberToken(string text) : base(text)
        {
        }
    }
}
