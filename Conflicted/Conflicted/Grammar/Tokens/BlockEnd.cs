using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Conflicted.Grammar.Tokens
{
    sealed class BlockEndLexer : Lexer<BlockEndToken>
    {
        public override BlockEndToken Consume(Source source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!IsMatch(source))
            {
                return null;
            }

            return new BlockEndToken(source.Read().ToString());
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

            char next = source.Peek();

            if (next == '}') return true;

            return false;
        }
    }

    sealed class BlockEndToken : Token
    {
        public BlockEndToken(string text) : base(text)
        {
        }
    }
}
