using Conflicted.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Conflicted.ViewModel
{
    internal class DataCache : BaseViewModel
    {
        public int CountMods
        {
            get => countMods;
            set
            {
                countMods = value;
                OnPropertyChanged();
            }
        }

        public int DoneMods
        {
            get => doneMods;
            set
            {
                doneMods = value;
                OnPropertyChanged();
            }
        }

        private readonly ModRegistry modRegistry;
        private readonly GameData gameData;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly Dictionary<Mod, IEnumerable<ModFile>> overwrittenFiles = new Dictionary<Mod, IEnumerable<ModFile>>();
        private readonly Dictionary<Mod, IEnumerable<ModFile>> overwritingFiles = new Dictionary<Mod, IEnumerable<ModFile>>();
        private readonly Dictionary<Mod, IEnumerable<ModElement>> overwrittenElements = new Dictionary<Mod, IEnumerable<ModElement>>();
        private readonly Dictionary<Mod, IEnumerable<ModElement>> overWritingElements = new Dictionary<Mod, IEnumerable<ModElement>>();

        private readonly Dictionary<Mod, int> overwrittenFilesCount = new Dictionary<Mod, int>();
        private readonly Dictionary<Mod, int> overwritingFilesCount = new Dictionary<Mod, int>();
        private readonly Dictionary<Mod, int> overwrittenElementsCount = new Dictionary<Mod, int>();
        private readonly Dictionary<Mod, int> overwritingElementsCount = new Dictionary<Mod, int>();

        private readonly Dictionary<Mod, int> modsWithOverwrittenFilesCount = new Dictionary<Mod, int>();
        private readonly Dictionary<Mod, int> modsWithOverwritingFilesCount = new Dictionary<Mod, int>();
        private readonly Dictionary<Mod, int> modsWithOverwrittenElementsCount = new Dictionary<Mod, int>();
        private readonly Dictionary<Mod, int> modsWithOverwritingElementsCount = new Dictionary<Mod, int>();

        private int countMods;
        private int doneMods;

        public DataCache(ModRegistry modRegistry, GameData gameData)
        {
            this.modRegistry = modRegistry ?? throw new ArgumentNullException(nameof(modRegistry));
            this.gameData = gameData ?? throw new ArgumentNullException(nameof(gameData));

            Task.Run(BuildCache, cancellationTokenSource.Token);
        }

        public void CancelCaching()
        {
            cancellationTokenSource.Cancel();
        }

        public IEnumerable<ModFile> GetOverwrittenFilesFor(Mod mod)
        {
            lock (overwrittenFiles)
            {
                if (overwrittenFiles.ContainsKey(mod))
                {
                    return overwrittenFiles[mod];
                }
                else
                {
                    var result = modRegistry.ConflictedFiles
                        .Where(file => file.Mod != mod)
                        .Where(file => mod.Files.Contains(file))
                        .Where(file => gameData.ModsOrder.IndexOf(file.Mod.ID) < gameData.ModsOrder.IndexOf(mod.ID))
                        .OrderBy(file => file.Mod, gameData);
                    overwrittenFiles[mod] = result.ToList();
                    return result;
                }
            }
        }

        public IEnumerable<ModFile> GetOverwritingFilesFor(Mod mod)
        {
            lock (overwritingFiles)
            {
                if (overwritingFiles.ContainsKey(mod))
                {
                    return overwritingFiles[mod];
                }
                else
                {
                    var result = modRegistry.ConflictedFiles
                        .Where(file => file.Mod != mod)
                        .Where(file => mod.Files.Contains(file))
                        .Where(file => gameData.ModsOrder.IndexOf(file.Mod.ID) > gameData.ModsOrder.IndexOf(mod.ID))
                        .OrderBy(file => file.Mod, gameData);
                    overwritingFiles[mod] = result.ToList();
                    return result;
                }
            }
        }

        public IEnumerable<ModElement> GetOverwrittenElementsFor(Mod mod)
        {
            lock (overwrittenElements)
            {
                if (overwrittenElements.ContainsKey(mod))
                {
                    return overwrittenElements[mod];
                }
                else
                {
                    var result = modRegistry.ConflictedElements
                        .Where(element => element.File.Mod != mod)
                        .Where(element => mod.Elements.Contains(element))
                        .Where(element => gameData.ModsOrder.IndexOf(element.File.Mod.ID) < gameData.ModsOrder.IndexOf(mod.ID))
                        .OrderBy(element => element.File.Mod, gameData);
                    overwrittenElements[mod] = result.ToList();
                    return result;
                }
            }
        }

        public IEnumerable<ModElement> GetOverwritingElementsFor(Mod mod)
        {
            lock (overWritingElements)
            {
                if (overWritingElements.ContainsKey(mod))
                {
                    return overWritingElements[mod];
                }
                else
                {
                    var result = modRegistry.ConflictedElements
                        .Where(element => element.File.Mod != mod)
                        .Where(element => mod.Elements.Contains(element))
                        .Where(element => gameData.ModsOrder.IndexOf(element.File.Mod.ID) > gameData.ModsOrder.IndexOf(mod.ID))
                        .OrderBy(element => element.File.Mod, gameData);
                    overWritingElements[mod] = result.ToList();
                    return result;
                }
            }
        }

        public int GetOverwrittenFilesCountFor(Mod mod)
        {
            lock (overwrittenFilesCount)
            {
                if (overwrittenFilesCount.ContainsKey(mod))
                {
                    return overwrittenFilesCount[mod];
                }
                else
                {
                    int count = GetOverwrittenFilesFor(mod).Count();
                    overwrittenFilesCount[mod] = count;
                    return count;
                }
            }
        }

        public int GetOverwritingFilesCountFor(Mod mod)
        {
            lock (overwritingFilesCount)
            {
                if (overwritingFilesCount.ContainsKey(mod))
                {
                    return overwritingFilesCount[mod];
                }
                else
                {
                    int count = GetOverwritingFilesFor(mod).Count();
                    overwritingFilesCount[mod] = count;
                    return count;
                }
            }
        }

        public int GetOverwrittenElementsCountFor(Mod mod)
        {
            lock (overwrittenElementsCount)
            {
                if (overwrittenElementsCount.ContainsKey(mod))
                {
                    return overwrittenElementsCount[mod];
                }
                else
                {
                    int count = GetOverwrittenElementsFor(mod).Count();
                    overwrittenElementsCount[mod] = count;
                    return count;
                }
            }
        }

        public int GetOverwritingElementsCountFor(Mod mod)
        {
            lock (overwritingElementsCount)
            {
                if (overwritingElementsCount.ContainsKey(mod))
                {
                    return overwritingElementsCount[mod];
                }
                else
                {
                    int count = GetOverwritingElementsFor(mod).Count();
                    overwritingElementsCount[mod] = count;
                    return count;
                }
            }
        }

        public int GetModsWithOverwrittenFilesCountFor(Mod mod)
        {
            lock (modsWithOverwrittenFilesCount)
            {
                if (modsWithOverwrittenFilesCount.ContainsKey(mod))
                {
                    return modsWithOverwrittenFilesCount[mod];
                }
                else
                {
                    int count = GetOverwrittenFilesFor(mod).Select(file => file.Mod).Distinct().Count();
                    modsWithOverwrittenFilesCount[mod] = count;
                    return count;
                }
            }
        }

        public int GetModsWithOverwritingFilesCountFor(Mod mod)
        {
            lock (modsWithOverwritingFilesCount)
            {
                if (modsWithOverwritingFilesCount.ContainsKey(mod))
                {
                    return modsWithOverwritingFilesCount[mod];
                }
                else
                {
                    int count = GetOverwritingFilesFor(mod).Select(file => file.Mod).Distinct().Count();
                    modsWithOverwritingFilesCount[mod] = count;
                    return count;
                }
            }
        }

        public int GetModsWithOverwrittenElementsCountFor(Mod mod)
        {
            lock (modsWithOverwrittenElementsCount)
            {
                if (modsWithOverwrittenElementsCount.ContainsKey(mod))
                {
                    return modsWithOverwrittenElementsCount[mod];
                }
                else
                {
                    int count = GetOverwrittenElementsFor(mod).Select(element => element.File.Mod).Distinct().Count();
                    modsWithOverwrittenElementsCount[mod] = count;
                    return count;
                }
            }
        }

        public int GetModsWithOverwritingElementsCountFor(Mod mod)
        {
            lock (modsWithOverwritingElementsCount)
            {
                if (modsWithOverwritingElementsCount.ContainsKey(mod))
                {
                    return modsWithOverwritingElementsCount[mod];
                }
                else
                {
                    int count = GetOverwritingElementsFor(mod).Select(element => element.File.Mod).Distinct().Count();
                    modsWithOverwritingElementsCount[mod] = count;
                    return count;
                }
            }
        }

        private void BuildCache()
        {
            CancellationToken token = cancellationTokenSource.Token;

            CountMods = modRegistry.Count;

            foreach (var mod in modRegistry.Values)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                GetOverwrittenFilesFor(mod);
                GetOverwritingFilesFor(mod);
                GetOverwrittenElementsFor(mod);
                GetOverwritingElementsFor(mod);

                GetOverwrittenFilesCountFor(mod);
                GetOverwritingFilesCountFor(mod);
                GetOverwrittenElementsCountFor(mod);
                GetOverwritingElementsCountFor(mod);

                GetModsWithOverwrittenFilesCountFor(mod);
                GetModsWithOverwritingFilesCountFor(mod);
                GetModsWithOverwrittenElementsCountFor(mod);
                GetModsWithOverwritingElementsCountFor(mod);

                DoneMods++;
            }
        }
    }
}