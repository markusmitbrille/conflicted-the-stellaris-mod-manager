using System;
using System.Collections.Generic;

namespace Conflicted.Model
{
    internal class ModRegistry : Dictionary<string, Mod>
    {
        private readonly List<Conflict> fileConflicts = new List<Conflict>();
        private readonly List<Conflict> elementConflicts = new List<Conflict>();

        public IReadOnlyList<Conflict> FileConflicts => fileConflicts;
        public IReadOnlyList<Conflict> ElementConflicts => elementConflicts;

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