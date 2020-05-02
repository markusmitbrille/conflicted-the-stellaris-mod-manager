using System.IO;

namespace Conflicted.Grammar
{
    interface ITokenizer<T> where T : Token
    {
        public bool IsMatch(StringReader source);
        public T Consume(StringReader source);
    }
}
