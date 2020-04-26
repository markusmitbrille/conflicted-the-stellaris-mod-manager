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
        public int Level { get; private set; }

        public ElementListener(ModFile file, int level)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            Level = level;
        }

        protected ElementListener(ModFile file) : this(file, 0)
        {
        }

        public override void EnterGroup([NotNull] StellarisParser.GroupContext context)
        {
            Level++;
        }

        public override void ExitGroup([NotNull] StellarisParser.GroupContext context)
        {
            Level--;
        }

        protected void AddElement(ModElement element)
        {
            elements.Add(element);
        }
    }
}