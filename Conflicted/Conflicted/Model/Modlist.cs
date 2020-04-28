using Conflicted.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Conflicted.Model
{
    class Modlist
    {
        private List<Mod> mods = new List<Mod>();
        public IEnumerable<Mod> Mods => mods?.OrderBy(mod => mod, Mod.OrderComparer.Instance);

        private List<string> order = new List<string>();
        public ReadOnlyCollection<string> Order => order?.AsReadOnly();

        private IEnumerable<IGrouping<string, ModFile>> fileConflicts;
        public IEnumerable<IGrouping<string, ModFile>> FileConflicts
        {
            get
            {
                return fileConflicts ?? (fileConflicts = mods
                    .SelectMany(mod => mod.Files)
                    .Where(file => !Settings.Default.IgnoredFiles.Contains(file.NameWithExtension))
                    .GroupBy(file => file.ID)
                    .Where(group => group.Count() > 1)
                    .ToArray());
            }
        }

        private IEnumerable<IGrouping<string, ModElement>> elementConflicts;
        public IEnumerable<IGrouping<string, ModElement>> ElementConflicts
        {
            get
            {
                return elementConflicts ?? (elementConflicts = mods
                    .SelectMany(mod => mod.Elements)
                    .GroupBy(element => element.ID)
                    .Where(group => group.Count() > 1)
                    .ToArray());
            }
        }

        public event EventHandler RegistryLoading;
        public event EventHandler RegistryLoaded;

        public event EventHandler DataLoading;
        public event EventHandler DataLoaded;

        public event EventHandler<ModMovedEventArgs> ModMovedTop;
        public event EventHandler<ModMovedEventArgs> ModMovedUp;
        public event EventHandler<ModMovedEventArgs> ModMovedDown;
        public event EventHandler<ModMovedEventArgs> ModMovedBottom;

        public Modlist()
        {
        }

        public static void MoveTop(Mod mod)
        {
            if (mod.ID == mod.Modlist.order.First())
            {
                return;
            }

            mod.Modlist.order.Remove(mod.ID);
            mod.Modlist.order.Insert(0, mod.ID);

            mod.Modlist.ModMovedTop?.Invoke(mod.Modlist, new ModMovedEventArgs(mod));
        }

        public static void MoveUp(Mod mod)
        {
            if (mod.ID == mod.Modlist.order.First())
            {
                return;
            }

            int index = mod.Modlist.order.IndexOf(mod.ID);
            mod.Modlist.order.Remove(mod.ID);
            mod.Modlist.order.Insert(index - 1, mod.ID);

            mod.Modlist.ModMovedUp?.Invoke(mod.Modlist, new ModMovedEventArgs(mod));
        }

        public static void MoveDown(Mod mod)
        {
            if (mod.ID == mod.Modlist.order.Last())
            {
                return;
            }

            int index = mod.Modlist.order.IndexOf(mod.ID);
            mod.Modlist.order.Remove(mod.ID);
            mod.Modlist.order.Insert(index + 1, mod.ID);

            mod.Modlist.ModMovedDown?.Invoke(mod.Modlist, new ModMovedEventArgs(mod));
        }

        public static void MoveBottom(Mod mod)
        {
            if (mod.ID == mod.Modlist.order.Last())
            {
                return;
            }

            mod.Modlist.order.Remove(mod.ID);
            mod.Modlist.order.Add(mod.ID);

            mod.Modlist.ModMovedBottom?.Invoke(mod.Modlist, new ModMovedEventArgs(mod));
        }

        public bool OpenModRegistry(string file)
        {
            if (!File.Exists(file))
            {
                return false;
            }

            ModRegistry registry;
            try
            {
                registry = JsonConvert.DeserializeObject<ModRegistry>(File.ReadAllText(file));
            }
            catch (Exception)
            {
                return false;
            }

            if (registry == null)
            {
                return false;
            }

            Load(registry);

            return true;
        }

        public bool OpenGameData(string file)
        {
            if (!File.Exists(file))
            {
                return false;
            }

            GameData data;
            try
            {
                data = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(file));
            }
            catch (Exception)
            {
                return false;
            }
            
            if (data == null)
            {
                return false;
            }

            Load(data);

            return true;
        }

        public void SaveModRegistry(string file)
        {
            if (File.Exists(file))
            {
                File.Copy(file, $"{Path.GetDirectoryName(file)}{Path.DirectorySeparatorChar}{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileName(file)}.bak", true);
            }

            ModRegistry registry = new ModRegistry(mods.ToDictionary(mod => mod.ID, mod => new ModRegistryEntry()
            {
                SteamID = mod.SteamID,
                DisplayName = mod.DisplayName,
                Tags = mod.Tags.ToList(),
                TimeUpdated = mod.TimeUpdated,
                Source = mod.Source,
                ThumbnailUrl = mod.ThumbnailUrl,
                DirPath = mod.DirPath,
                Status = mod.Status,
                ID = mod.ID,
                GameRegistryId = mod.GameRegistryId,
                RequiredVersion = mod.RequiredVersion,
                ArchivePath = mod.ArchivePath,
                Cause = mod.Cause,
                ThumbnailPath = mod.ThumbnailPath,
            }));

            File.WriteAllText(file, JsonConvert.SerializeObject(registry, Formatting.Indented, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
        }

        public void SaveGameData(string file)
        {
            if (File.Exists(file))
            {
                File.Copy(file, $"{file}.bak{DateTime.Now:yyyyMMddHHmmss}", true);
            }

            GameData data = new GameData()
            {
                IsEulaAccepted = true,
                ModsOrder = order.ToList()
            };

            File.WriteAllText(file, JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
        }

        private void Load(ModRegistry registry)
        {
            RegistryLoading?.Invoke(this, EventArgs.Empty);

            mods = registry?.Values.Select(entry => new Mod(this, entry)).ToList() ?? new List<Mod>();

            fileConflicts = null;
            elementConflicts = null;

            foreach (var mod in mods.Where(mod => !order.Contains(mod.ID)))
            {
                order.Insert(0, mod.ID);
            }

            RegistryLoaded?.Invoke(this, EventArgs.Empty);
        }

        private void Load(GameData data)
        {
            DataLoading?.Invoke(this, EventArgs.Empty);

            order = data?.ModsOrder.ToList() ?? new List<string>();

            foreach (var mod in mods.Where(mod => !order.Contains(mod.ID)))
            {
                order.Insert(0, mod.ID);
            }

            DataLoaded?.Invoke(this, EventArgs.Empty);
        }

        public class ModMovedEventArgs : EventArgs
        {
            public Mod Mod { get; }

            public ModMovedEventArgs(Mod mod)
            {
                Mod = mod;
            }
        }
    }
}