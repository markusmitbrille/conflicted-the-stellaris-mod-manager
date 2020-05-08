using System;
using System.Collections.Generic;
using System.Text;

namespace Conflicted.Grammar
{
    class Source : Reader<char>
    {
        public Source(string source) : base(source.ToCharArray())
        {
        }

        public Source(Source reader) : base(reader)
        {
        }
    }
}
