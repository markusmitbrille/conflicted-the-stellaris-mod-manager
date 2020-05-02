using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Conflicted.Models
{
    [DataContract]
    class ModRegistryEntry
    {
        [DataMember(Name = "steamId")]
        public long? SteamID { get; set; }

        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "tags")]
        public List<string> Tags { get; set; }

        [DataMember(Name = "timeUpdated")]
        public long? TimeUpdated { get; set; }

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
    }
}