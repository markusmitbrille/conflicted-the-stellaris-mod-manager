using System.Collections.Generic;

namespace Conflicted.Grammar
{
    class Book : List<Token>
    {
        public Book()
        {
        }

        public Book(IEnumerable<Token> collection) : base(collection)
        {
        }

        public Book(int capacity) : base(capacity)
        {
        }
    }
}
