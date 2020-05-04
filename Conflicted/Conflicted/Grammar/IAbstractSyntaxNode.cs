namespace Conflicted.Grammar
{
    interface IAbstractSyntaxNode
    {
        public string Text { get; }
        public bool IsRoot { get; }
        public bool IsInternal { get; }
        public bool IsLeaf { get; }
    }
}
