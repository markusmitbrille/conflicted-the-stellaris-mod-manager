using System;

namespace Conflicted.Grammar
{
    abstract class Token
    {
        public string Text { get; private set; }

        protected Token(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }
    }
}
