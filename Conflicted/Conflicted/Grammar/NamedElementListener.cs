using Antlr4.Runtime.Misc;
using Conflicted.Model;

namespace Conflicted.Grammar
{
    class NamedElementListener : ElementListener
    {
        public int NameLevel { get; private set; } = 0;

        public NamedElementListener(ModFile file) : base(file)
        {
        }

        public NamedElementListener(ModFile file, int nameLevel) : this(file)
        {
            NameLevel = nameLevel;
        }

        public override void EnterExpr([NotNull] StellarisParser.ExprContext context)
        {
            if (CurrentLevel != NameLevel)
            {
                return;
            }

            StellarisParser.KeyContext key = context.key();
            if (key == null)
            {
                return;
            }

            StellarisParser.GroupContext group = context.val()?.group();
            if (group == null)
            {
                return;
            }

            string name = key.ToString();
            string text = context.ToString();
            string id = $"{File.Namespace}::{name}";
            AddElement(new ModElement(File, id, name, text));
        }
    }
}