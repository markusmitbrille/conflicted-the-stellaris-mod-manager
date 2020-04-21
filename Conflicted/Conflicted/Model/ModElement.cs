using System;

namespace Conflicted.Model
{
    internal class ModElement : IEquatable<ModElement>
    {
        public ModFile File { get; }
        public string Name { get; }
        public string Text { get; }

        public ModElement(ModFile file, string name, string text)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is ModElement ? Equals((ModElement)obj) : false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool Equals(ModElement other)
        {
            return Name.Equals(other.Name);
        }
    }
}