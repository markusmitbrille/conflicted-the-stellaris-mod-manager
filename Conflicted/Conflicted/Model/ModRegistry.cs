using System;
using System.Collections.Generic;

namespace Conflicted.Model
{
    internal class ModRegistry : Dictionary<string, Mod>
    {
        private readonly Dictionary<Mod, List<string>> fileConflicts;
        public IReadOnlyDictionary<Mod, List<string>> FileConflicts => fileConflicts;

        private readonly Dictionary<Mod, List<string>> elementConflicts;
        public IReadOnlyDictionary<Mod, List<string>> ElementConflicts => elementConflicts;

        public void FindFileConflicts()
        {
            throw new NotImplementedException();
        }

        public void FindElementConflicts()
        {
            throw new NotImplementedException();
        }
    }
}