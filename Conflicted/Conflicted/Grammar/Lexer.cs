using System.IO;

namespace Conflicted.Grammar
{
    abstract class Lexer<T> : ILexer<T>, ILexer where T : Token
    {
        public abstract T Consume(SourceReader source);
        public abstract bool IsMatch(SourceReader source);

        Token ILexer.Consume(SourceReader source)
        {
            return Consume(source);
        }
    }
}
