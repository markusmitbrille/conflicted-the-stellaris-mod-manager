using System;
using System.Collections.Generic;

namespace Conflicted.Model
{
    internal class Conflict
    {
        private readonly List<Mod> conflictors;

        public Mod Mod { get; }
        public string File { get; }
        public string Text { get; }

        public IReadOnlyList<Mod> Conflictors => conflictors;

        public Conflict(Mod mod, string file, string text, List<Mod> conflictors)
        {
            Mod = mod ?? throw new ArgumentNullException(nameof(mod));
            File = file ?? throw new ArgumentNullException(nameof(file));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            this.conflictors = conflictors ?? throw new ArgumentNullException(nameof(conflictors));
        }
    }
}