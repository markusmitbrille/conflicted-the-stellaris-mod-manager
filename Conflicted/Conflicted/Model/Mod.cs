﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Conflicted.Model
{
    [DataContract]
    internal class Mod
    {
        [DataMember(Name = "archivePath")]
        public string ArchivePath { get; set; }

        [DataMember(Name = "cause")]
        public string Cause { get; set; }

        [DataMember(Name = "dirPath")]
        public string DirPath { get; set; }

        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "gameRegistryId")]
        public string GameRegistryId { get; set; }

        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name = "requiredVersion")]
        public string RequiredVersion { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "steamId")]
        public long SteamID { get; set; }

        [DataMember(Name = "tags")]
        public List<string> Tags { get; set; }

        [DataMember(Name = "thumbnailPath")]
        public string ThumbnailPath { get; set; }

        [DataMember(Name = "thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        [DataMember(Name = "timeUpdated")]
        public long TimeUpdated { get; set; }

        private const string SourceSteam = "steam";

        public string WebPageUrl
        {
            get
            {
                switch (Source)
                {
                    case SourceSteam:
                        return $"https://steamcommunity.com/sharedfiles/filedetails/?id={SteamID}";

                    default:
                        return null;
                }
            }
        }

        private readonly Dictionary<string, string> files = new Dictionary<string, string>();
        public IReadOnlyDictionary<string, string> Files => files;

        private readonly Dictionary<string, string> elements = new Dictionary<string, string>();
        public IReadOnlyDictionary<string, string> Elements => elements;

        public void ReadFiles()
        {
            throw new NotImplementedException();
        }

        public void ReadElements()
        {
            throw new NotImplementedException();
        }
    }
}