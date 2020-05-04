namespace Conflicted.Grammar
{
    interface IParser<out T> where T : IAbstractSyntaxTree
    {
        public abstract T Consume(BookReader source);
        public abstract bool IsMatch(BookReader source);
    }

    interface IParser
    {
        public abstract IAbstractSyntaxTree Consume(BookReader source);
        public abstract bool IsMatch(BookReader source);
    }
}
