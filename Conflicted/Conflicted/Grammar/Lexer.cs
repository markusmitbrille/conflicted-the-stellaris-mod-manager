using System.IO;

namespace Conflicted.Grammar
{
    abstract class Lexer<T> : ILexer<T>, ILexer where T : Token
    {
        public abstract T Consume(Source source);
        public abstract bool IsMatch(Source source);

        Token ILexer.Consume(Source source)
        {
            return Consume(source);
        }
    }
}
