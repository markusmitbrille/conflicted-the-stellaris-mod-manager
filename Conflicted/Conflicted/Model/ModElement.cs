using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Conflicted.Model
{
    internal class ModElement
    {
        public string ID { get; }
        public string Text { get; }

        public ModFile File { get; }
        public Mod Mod => File.Mod;

        private IEnumerable<ModElement> conflicts;
        public IEnumerable<ModElement> Conflicts
        {
            get
            {
                return conflicts ?? (conflicts = File.Mod.Modlist.ElementConflicts.Where(group => group.Key == ID)
                    .SelectMany(group => group)
                    .Where(element => element != this)
                    .ToArray());
            }
        }

        private IEnumerable<ModElement> overwritten;
        public IEnumerable<ModElement> Overwritten => overwritten ?? (overwritten = Conflicts.Where(element => Mod.OrderComparer.Instance.Compare(element.Mod, Mod) < 0).ToArray());

        private IEnumerable<ModElement> overwriting;
        public IEnumerable<ModElement> Overwriting => overwriting ?? (overwriting = Conflicts.Where(element => Mod.OrderComparer.Instance.Compare(element.Mod, Mod) > 0).ToArray());

        public ModElement(ModFile file, string name, string text)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            ID = name ?? throw new ArgumentNullException(nameof(name));
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public override string ToString()
        {
            return ID;
        }
    }
}