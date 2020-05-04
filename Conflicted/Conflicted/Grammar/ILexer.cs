using System.IO;

namespace Conflicted.Grammar
{
    interface ILexer<out T> where T : Token
    {
        public bool IsMatch(SourceReader source);
        public T Consume(SourceReader source);
    }

    interface ILexer
    {
        public bool IsMatch(SourceReader source);
        public Token Consume(SourceReader source);
    }
}
