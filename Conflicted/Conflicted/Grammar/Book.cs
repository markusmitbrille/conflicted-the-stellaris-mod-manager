using System.Collections.Generic;

namespace Conflicted.Grammar
{
    class Book : List<IToken>
    {
        public Book()
        {
        }

        public Book(IEnumerable<IToken> collection) : base(collection)
        {
        }

        public Book(int capacity) : base(capacity)
        {
        }
    }
}
