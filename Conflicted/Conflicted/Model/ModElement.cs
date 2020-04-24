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
                return conflicts ?? (conflicts = File.Mod.Modlist.ElementConflicts
                    .Where(group => group.Key == ID)
                    .SelectMany(group => group)
                    .Where(element => element != this)
                    .ToArray());
            }
        }

        public ModElement(ModFile file, string id, string text)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            ID = id ?? throw new ArgumentNullException(nameof(id));
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public override string ToString()
        {
            return ID;
        }
    }
}