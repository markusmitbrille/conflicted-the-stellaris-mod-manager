using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Conflicted.Grammar.Tokens
{
    sealed class AccessorLexer : Lexer<AccessorToken>
    {
        public override AccessorToken Consume(SourceReader source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!IsMatch(source))
            {
                return null;
            }

            return new AccessorToken(source.Read().ToString());
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

            if (next == '.') return true;
            if (next == ':') return true;
            if (next == '@') return true;

            return false;
        }
    }

    sealed class AccessorToken : Token
    {
        public AccessorToken(string text) : base(text)
        {
        }
    }
}
