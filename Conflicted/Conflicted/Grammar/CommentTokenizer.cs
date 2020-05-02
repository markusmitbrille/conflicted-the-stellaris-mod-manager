using System;
using System.IO;

namespace Conflicted.Grammar
{
    class CommentTokenizer : ITokenizer<CommentToken>
    {
        public CommentToken Consume(StringReader source)
        {
            throw new NotImplementedException();
        }

        public bool IsMatch(StringReader source)
        {
            throw new NotImplementedException();
        }
    }
}
