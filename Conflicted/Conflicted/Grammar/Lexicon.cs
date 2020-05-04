using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Conflicted.Grammar
{
    class Lexicon : List<ILexer>
    {
        public Lexicon()
        {
        }

        public Lexicon(IEnumerable<ILexer> collection) : base(collection)
        {
        }

        public Lexicon(int capacity) : base(capacity)
        {
        }

        public Book Lex(SourceReader source)
        {
            Book tokens = new Book();

            while (this.Any(lexer=>lexer.IsMatch(source)))
            {
                tokens.Add(this.First(lexer => lexer.IsMatch(source)).Consume(source));
            }

            return tokens;
        }
    }
}
