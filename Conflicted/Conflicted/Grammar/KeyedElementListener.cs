using Antlr4.Runtime.Misc;
using Conflicted.Model;
using System;

namespace Conflicted.Grammar
{
    class KeyedElementListener : ElementListener
    {
        public int ElementLevel { get; private set; } = 0;
        public int KeyLevel { get; private set; } = 1;
        public string KeyName { get; private set; }

        private StellarisParser.ExprContext element;

        public KeyedElementListener(ModFile file, string keyName) : base(file)
        {
            KeyName = keyName ?? throw new ArgumentNullException(nameof(keyName));
        }

        public KeyedElementListener(ModFile file, int elementLevel, int keyLevel, string keyName) : this(file, keyName)
        {
            ElementLevel = elementLevel;
            KeyLevel = keyLevel;
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
                if (CurrentLevel != KeyLevel)
                {
                    return;
                }

                if (element == null)
                {
                    return;
                }

                StellarisParser.KeyContext key = context.key();
                if (key == null)
                {
                    return;
                }

                string keyString = key.ToString();
                if (keyString != KeyName)
                {
                    return;
                }

                StellarisParser.ValContext val = context.val();
                if (val == null)
                {
                    return;
                }

                string name = val.ToString();
                string text = element.ToString();
                string id = $"{File.Namespace}::{name}";
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