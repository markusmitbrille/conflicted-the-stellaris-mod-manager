using System;
using System.Collections.Generic;

namespace Conflicted.Grammar
{
    interface IReader<T>
    {
        public bool CanPeek();
        public T Peek();

        public bool CanPeek(int delta);
        public T Peek(int delta);

        public bool CanRead();
        public T Read();
    }
}
