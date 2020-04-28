using Antlr4.Runtime.Misc;
using Conflicted.Model;
using System;
using System.Collections.Generic;

namespace Conflicted.Grammar
{
    abstract class ElementListener : StellarisBaseListener
    {
        private List<ModElement> elements = new List<ModElement>();
        public IEnumerable<ModElement> Elements => elements.AsReadOnly();

        public ModFile File { get; private set; }

        protected int CurrentLevel { get; private set; } = 0;

        public ElementListener(ModFile file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        public override void EnterGroup([NotNull] StellarisParser.GroupContext context)
        {
            CurrentLevel++;
        }

        public override void ExitGroup([NotNull] StellarisParser.GroupContext context)
        {
            CurrentLevel--;
        }

        protected void AddElement(ModElement element)
        {
            elements.Add(element);
        }
    }
}