using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Conflicted.Model
{
    class Mod
    {
        public long? SteamID { get; }
        public string DisplayName { get; }
        public ReadOnlyCollection<string> Tags { get; }
        public long? TimeUpdated { get; }
        public string Source { get; }
        public string ThumbnailUrl { get; }
        public string DirPath { get; }
        public string Status { get; }
        public string ID { get; }
        public string GameRegistryId { get; }
        public string RequiredVersion { get; }
        public string ArchivePath { get; }
        public string Cause { get; }
        public string ThumbnailPath { get; }

        public ModList Modlist { get; }

        private IEnumerable<ModFile> files;
        public IEnumerable<ModFile> Files => files ?? (files = Directory.GetFiles(DirPath, "*", SearchOption.AllDirectories).Select(path => new ModFile(this, path)).ToList().AsReadOnly());

        private IEnumerable<ModElement> elements;
        public IEnumerable<ModElement> Elements => elements ?? (elements = Files.SelectMany(file => file.Elements).ToList().AsReadOnly());

        public Mod(ModList modlist, ModRegistryEntry entry)
        {
            Modlist = modlist ?? throw new ArgumentNullException(nameof(modlist));

            if (entry is null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            SteamID = entry.SteamID;
            DisplayName = entry.DisplayName;
            Tags = entry.Tags?.AsReadOnly();
            TimeUpdated = entry.TimeUpdated;
            Source = entry.Source;
            ThumbnailUrl = entry.ThumbnailUrl;
            DirPath = entry.DirPath;
            Status = entry.Status;
            ID = entry.ID;
            GameRegistryId = entry.GameRegistryId;
            RequiredVersion = entry.RequiredVersion;
            ArchivePath = entry.ArchivePath;
            Cause = entry.Cause;
            ThumbnailPath = entry.ThumbnailPath;
        }

        public void MoveTop() => ModList.MoveTop(this);

        public void MoveUp() => ModList.MoveUp(this);

        public void MoveDown() => ModList.MoveDown(this);

        public void MoveBottom() => ModList.MoveBottom(this);

        public override string ToString()
        {
            return DisplayName;
        }

        public class OrderComparer : Comparer<Mod>
        {
            public static OrderComparer Instance => instance ?? (instance = new OrderComparer());
            private static OrderComparer instance;

            private OrderComparer()
            {
            }

            public override int Compare(Mod x, Mod y)
            {
                int xIndex = x.Modlist.Order.IndexOf(x.ID);
                int yIndex = y.Modlist.Order.IndexOf(y.ID);

                return xIndex == yIndex ? 0 : xIndex == -1 ? -1 : yIndex == -1 ? 1 : xIndex - yIndex;
            }
        }
    }
}