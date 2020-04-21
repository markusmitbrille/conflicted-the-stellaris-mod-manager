using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Conflicted.Model
{
    [DataContract]
    internal class Mod : IEquatable<Mod>
    {
        private const string SourceSteam = "steam";

        private readonly List<ModFile> files = new List<ModFile>();
        private readonly List<ModElement> elements = new List<ModElement>();

        [DataMember(Name = "steamId")]
        public long SteamID { get; set; }

        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "tags")]
        public List<string> Tags { get; set; }

        [DataMember(Name = "timeUpdated")]
        public long TimeUpdated { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        [DataMember(Name = "thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        [DataMember(Name = "dirPath")]
        public string DirPath { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name = "gameRegistryId")]
        public string GameRegistryId { get; set; }

        [DataMember(Name = "requiredVersion")]
        public string RequiredVersion { get; set; }

        [DataMember(Name = "archivePath")]
        public string ArchivePath { get; set; }

        [DataMember(Name = "cause")]
        public string Cause { get; set; }

        [DataMember(Name = "thumbnailPath")]
        public string ThumbnailPath { get; set; }

        public string WebPageUrl
        {
            get
            {
                switch (Source)
                {
                    case SourceSteam:
                        return $"https://steamcommunity.com/sharedfiles/filedetails/?id={SteamID}";

                    default:
                        return "https://www.google.com";
                }
            }
        }

        public IReadOnlyList<ModFile> Files => files;
        public IReadOnlyList<ModElement> Elements => elements;

        public override string ToString()
        {
            return DisplayName;
        }

        public override bool Equals(object obj)
        {
            return obj is Mod ? Equals((Mod)obj) : false;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public bool Equals(Mod other)
        {
            return ID.Equals(other.ID);
        }

        [OnDeserialized]
        private void ReadFiles(StreamingContext context)
        {
            foreach (var path in Directory.GetFiles(DirPath, "*", SearchOption.AllDirectories))
            {
                ModFile file = new ModFile(this, path);
                files.Add(file);
                elements.AddRange(file.Elements);
            }
        }
    }
}