using Antlr4.Runtime.Misc;
using Conflicted.Model;

namespace Conflicted.Grammar
{
    class KeyValueElementListener : ElementListener
    {
        public int ElementLevel { get; private set; } = 0;
        public int KeyValueLevel { get; private set; } = 1;

        private StellarisParser.ExprContext element;

        public KeyValueElementListener(ModFile file) : base(file)
        {
        }

        public KeyValueElementListener(ModFile file, int elementLevel, int keyValueLevel) : this(file)
        {
            ElementLevel = elementLevel;
            KeyValueLevel = keyValueLevel;
        }

        public override void EnterExpr([NotNull] StellarisParser.ExprContext context)
        {
            SetElement();
            ParseElement();

            void SetElement()
            {
                if (CurrentLevel == ElementLevel)
                {
                    element = context;
                }
            }
            void ParseElement()
            {
                if (CurrentLevel != KeyValueLevel)
                {
                    return;
                }

                if (element == null)
                {
                    return;
                }

                StellarisParser.KeyContext elementKey = element.key();
                if (elementKey == null)
                {
                    return;
                }

                StellarisParser.KeyContext key = context.key();
                if (key == null)
                {
                    return;
                }

                StellarisParser.ValContext val = context.val();
                if (val == null)
                {
                    return;
                }

                string name = val.ToString();
                string text = context.ToString();
                string id = $"{File.Namespace}::{elementKey}::{name}";
                AddElement(new ModElement(File, id, name, text));
            }
        }

        public override void ExitExpr([NotNull] StellarisParser.ExprContext context)
        {
            SetElement();

            void SetElement()
            {
                if (CurrentLevel == ElementLevel)
                {
                    element = null;
                }
            }
        }
    }
}