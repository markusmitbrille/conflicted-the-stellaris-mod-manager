using System;
using System.Collections.Generic;
using System.Text;

namespace Conflicted.Grammar
{
    class SourceReader : Reader<char>
    {
        public SourceReader(string source) : base(source.ToCharArray())
        {
        }

        public SourceReader(SourceReader reader) : base(reader)
        {
        }
    }
}
