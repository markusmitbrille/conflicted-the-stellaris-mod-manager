using System;
using System.Collections.Generic;
using System.IO;

namespace Conflicted.Model
{
    internal class ModFile : IEquatable<ModFile>
    {
        private readonly List<ModElement> elements = new List<ModElement>();
        public IReadOnlyList<ModElement> Elements => elements;

        public Mod Mod { get; }
        public string Path { get; }
        public string ID { get; }
        public string Name { get; }
        public string Extension { get; }
        public string Directory { get; }

        public ModFile(Mod mod, string path)
        {
            Mod = mod ?? throw new ArgumentNullException(nameof(mod));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            ID = path.Remove(0, mod.DirPath.Length + 1);
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            Extension = System.IO.Path.GetExtension(path);
            Directory = new DirectoryInfo(path).Name;

            ReadElements();
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is ModFile ? Equals((ModFile)obj) : false;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public bool Equals(ModFile other)
        {
            return ID.Equals(other.ID);
        }

        private void ReadElements()
        {
            if (Extension != ".txt")
            {
                return;
            }

            string text = File.ReadAllText(Path);

            Mode mode = Mode.Code;
            int level = 0, wordStart = 0, wordEnd = 0, elementStart = 0, elementEnd = 0, elementNameStart = 0, elementNameEnd = 0;

            for (int i = 0; i < text.Length; i++)
            {
                switch (mode)
                {
                    case Mode.Code:
                        if (text[i] == '#')
                        {
                            mode = Mode.Comment;
                        }
                        else if (text[i] == '"')
                        {
                            mode = Mode.String;
                        }
                        else if (text[i] == '=')
                        {
                            if (level == 0)
                            {
                                elementNameStart = wordStart;
                                elementNameEnd = wordEnd;
                                elementStart = wordStart;
                            }
                        }
                        else if (text[i] == '{')
                        {
                            level++;
                        }
                        else if (text[i] == '}')
                        {
                            level--;

                            if (level == 0)
                            {
                                elementEnd = i;
                                elements.Add(new ModElement(this, text.Substring(elementNameStart, elementNameEnd - elementNameStart), text.Substring(elementStart, elementEnd - elementStart)));
                            }
                        }
                        else if (!char.IsWhiteSpace(text[i]))
                        {
                            mode = Mode.Word;
                            wordStart = i;
                        }
                        break;

                    case Mode.Word:
                        if (char.IsWhiteSpace(text[i]))
                        {
                            mode = Mode.Code;
                            wordEnd = i;
                        }
                        break;

                    case Mode.String:
                        if (text[i] == '"' && text[i - 1] != '\\')
                        {
                            mode = Mode.Code;
                        }
                        break;

                    case Mode.Comment:
                        if (text[i] == '\n')
                        {
                            mode = Mode.Code;
                        }
                        break;
                }
            }
        }

        private enum Mode
        {
            Code,
            Word,
            String,
            Comment
        }
    }
}