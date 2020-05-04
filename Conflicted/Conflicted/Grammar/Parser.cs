using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conflicted.Grammar
{
    abstract class Parser<T> : IParser<T>, IParser where T : IAbstractSyntaxTree
    {
        public abstract T Consume(BookReader source);
        public abstract bool IsMatch(BookReader source);

        IAbstractSyntaxTree IParser.Consume(BookReader source)
        {
            return Consume(source);
        }
    }
}
