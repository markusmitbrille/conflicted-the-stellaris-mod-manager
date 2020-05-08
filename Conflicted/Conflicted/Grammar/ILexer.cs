using System.IO;

namespace Conflicted.Grammar
{
    interface ILexer<out T> where T : Token
    {
        public bool IsMatch(Source source);
        public T Consume(Source source);
    }

    interface ILexer
    {
        public bool IsMatch(Source source);
        public Token Consume(Source source);
    }
}
