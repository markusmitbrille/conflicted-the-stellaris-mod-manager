using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Conflicted.Grammar.Tokens
{
    sealed class CommentLexer : Lexer<CommentToken>
    {
        public override CommentToken Consume(Source source)
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

            while (source.CanPeek() &&
                char.GetUnicodeCategory(source.Peek()) != UnicodeCategory.ParagraphSeparator &&
                char.GetUnicodeCategory(source.Peek()) != UnicodeCategory.LineSeparator)
            {
                text.Append(source.Read());
            }

            return new CommentToken(text.ToString());
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

            return source.Peek() == '#';
        }
    }

    sealed class CommentToken : Token
    {
        public CommentToken(string text) : base(text)
        {
        }
    }
}
