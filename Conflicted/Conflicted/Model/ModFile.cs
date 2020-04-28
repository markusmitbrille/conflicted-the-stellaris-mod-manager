using Antlr4;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Conflicted.Grammar;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Conflicted.Model
{
    internal class ModFile
    {
        public string Path { get; }
        public string ID { get; }
        public string Name { get; }
        public string NameWithExtension { get; }
        public string Extension { get; }
        public string Directory { get; }
        public string Namespace { get; }
        public string Text { get; }

        public Mod Mod { get; }

        private IEnumerable<ModElement> elements;
        public IEnumerable<ModElement> Elements => elements ?? (elements = Parse().ToArray());

        private IEnumerable<ModFile> conflicts;
        public IEnumerable<ModFile> Conflicts
        {
            get
            {
                return conflicts ?? (conflicts = Mod.Modlist.FileConflicts.Where(group => group.Key == ID)
                    .SelectMany(group => group)
                    .Where(file => file != this)
                    .ToArray());
            }
        }

        public ModFile(Mod mod, string path)
        {
            Mod = mod ?? throw new ArgumentNullException(nameof(mod));
            Path = path ?? throw new ArgumentNullException(nameof(path));

            ID = path.Remove(0, mod.DirPath.Length + 1);
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            NameWithExtension = System.IO.Path.GetFileName(path);
            Extension = System.IO.Path.GetExtension(path);
            Directory = new DirectoryInfo(path).Parent.Name;
            Namespace = Directory;
            Text = Extension == ".txt" ? File.ReadAllText(path) : null;
        }

        public override string ToString()
        {
            return Name;
        }

        private IEnumerable<ModElement> Parse()
        {
            if (Text == null)
            {
                return Enumerable.Empty<ModElement>();
            }

            AntlrInputStream stream = new AntlrInputStream(Text);
            StellarisLexer lexer = new StellarisLexer(stream);
            CommonTokenStream tokenStream = new CommonTokenStream(lexer);
            StellarisParser parser = new StellarisParser(tokenStream);
            StellarisParser.ContentContext content = parser.content();

            List<ElementListener> listeners = new List<ElementListener>();
            switch (Directory)
            {
                case "ambient_objects":
                case "component_tags":
                case "global_ship_designs":
                case "on_actions":
                case "projectiles":
                case "ship_behaviors":
                case "technology":
                case "terraform":
                    break;

                case "defines":
                    listeners.Add(new KeyValueElementListener(this));
                    break;

                case "events":
                    listeners.Add(new KeyedElementListener(this, "id"));
                    break;

                case "component_sets":
                case "component_templates":
                case "special_projects":
                case "section_templates":
                    listeners.Add(new KeyedElementListener(this, "key"));
                    break;

                case "portraits":
                    listeners.Add(new NamedElementListener(this, 1));
                    break;

                default:
                    listeners.Add(new NamedElementListener(this));
                    break;
            }

            foreach (var listener in listeners)
            {
                ParseTreeWalker.Default.Walk(listener, content);
            }

            return listeners.SelectMany(listener => listener.Elements);
        }
    }
}