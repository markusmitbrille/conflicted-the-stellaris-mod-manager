using System.Collections.Generic;

namespace Conflicted.Model
{
    class ModRegistry : Dictionary<string, ModRegistryEntry>
    {
        public ModRegistry() : base()
        {
        }

        public ModRegistry(IDictionary<string, ModRegistryEntry> dictionary) : base(dictionary)
        {
        }
    }
}