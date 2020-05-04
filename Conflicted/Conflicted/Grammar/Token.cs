using System;

namespace Conflicted.Grammar
{
    abstract class Token : IToken
    {
        public string Text { get; private set; }

        public bool IsRoot => false;
        public bool IsInternal => false;
        public bool IsLeaf => true;

        protected Token(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }
    }
}
