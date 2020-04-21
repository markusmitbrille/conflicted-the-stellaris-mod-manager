using Conflicted.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Conflicted.ViewModel
{
    internal class MainWindowViewModel : BaseViewModel
    {
        public IEnumerable<Mod> Mods => gameData == null ? modRegistry?.Values.OrderBy(mod => mod.DisplayName) : modRegistry?.Values.OrderBy(mod => mod, gameData);

        public DataCache Cache
        {
            get => cache;
            set
            {
                cache = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ModFile> OverwrittenFiles
        {
            get
            {
                if (selectedMod == null)
                {
                    return null;
                }
                if (modRegistry == null)
                {
                    return null;
                }
                if (gameData == null)
                {
                    return null;
                }
                if (cache == null)
                {
                    return null;
                }

                return cache.GetOverwrittenFilesFor(selectedMod);
            }
        }

        public IEnumerable<ModFile> OverwritingFiles
        {
            get
            {
                if (selectedMod == null)
                {
                    return null;
                }
                if (modRegistry == null)
                {
                    return null;
                }
                if (gameData == null)
                {
                    return null;
                }
                if (cache == null)
                {
                    return null;
                }

                return cache.GetOverwritingFilesFor(selectedMod);
            }
        }

        public IEnumerable<ModElement> OverwrittenElements
        {
            get
            {
                if (selectedMod == null)
                {
                    return null;
                }
                if (modRegistry == null)
                {
                    return null;
                }
                if (gameData == null)
                {
                    return null;
                }
                if (cache == null)
                {
                    return null;
                }

                return cache.GetOverwrittenElementsFor(selectedMod);
            }
        }

        public IEnumerable<ModElement> OverwritingElements
        {
            get
            {
                if (selectedMod == null)
                {
                    return null;
                }
                if (modRegistry == null)
                {
                    return null;
                }
                if (gameData == null)
                {
                    return null;
                }
                if (cache == null)
                {
                    return null;
                }

                return cache.GetOverwritingElementsFor(selectedMod);
            }
        }

        public int OverwrittenFilesCount => selectedMod == null ? 0 : cache?.GetOverwrittenFilesCountFor(selectedMod) ?? 0;
        public int OverwritingFilesCount => selectedMod == null ? 0 : cache?.GetOverwritingFilesCountFor(selectedMod) ?? 0;
        public int OverwrittenElementsCount => selectedMod == null ? 0 : cache?.GetOverwrittenElementsCountFor(selectedMod) ?? 0;
        public int OverwritingElementsCount => selectedMod == null ? 0 : cache?.GetOverwritingElementsCountFor(selectedMod) ?? 0;

        public int ModsWithOverwrittenFilesCount => selectedMod == null ? 0 : cache?.GetModsWithOverwrittenFilesCountFor(selectedMod) ?? 0;
        public int ModsWithOverwritingFilesCount => selectedMod == null ? 0 : cache?.GetModsWithOverwritingFilesCountFor(selectedMod) ?? 0;
        public int ModsWithOverwrittenElementsCount => selectedMod == null ? 0 : cache?.GetModsWithOverwrittenElementsCountFor(selectedMod) ?? 0;
        public int ModsWithOverwritingElementsCount => selectedMod == null ? 0 : cache?.GetModsWithOverwritingElementsCountFor(selectedMod) ?? 0;

        public Mod SelectedMod
        {
            get => selectedMod;
            set
            {
                selectedMod = value;

                OnPropertyChanged();

                OnPropertyChanged(nameof(OverwrittenFiles));
                OnPropertyChanged(nameof(OverwritingFiles));
                OnPropertyChanged(nameof(OverwrittenElements));
                OnPropertyChanged(nameof(OverwritingElements));

                OnPropertyChanged(nameof(OverwrittenFilesCount));
                OnPropertyChanged(nameof(OverwritingFilesCount));
                OnPropertyChanged(nameof(OverwrittenElementsCount));
                OnPropertyChanged(nameof(OverwritingElementsCount));

                OnPropertyChanged(nameof(ModsWithOverwrittenFilesCount));
                OnPropertyChanged(nameof(ModsWithOverwritingFilesCount));
                OnPropertyChanged(nameof(ModsWithOverwrittenElementsCount));
                OnPropertyChanged(nameof(ModsWithOverwritingElementsCount));
            }
        }

        public RelayCommand OpenModRegistryCommand => openModRegistryCommand ?? (openModRegistryCommand = new RelayCommand(ExecuteOpenModRegistry));
        public RelayCommand OpenGameDataCommand => openGameDataCommand ?? (openGameDataCommand = new RelayCommand(ExecuteOpenGameData));
        public RelayCommand SaveGameDataCommand => saveGameDataCommand ?? (saveGameDataCommand = new RelayCommand(ExecuteSaveGameData, CanSaveGameData));
        public RelayCommand OpenOptionsCommand => openOptionsCommand ?? (openOptionsCommand = new RelayCommand(ExecuteOpenOptions));

        public RelayCommand<Mod> MoveModTopCommand => moveModTopCommand ?? (moveModTopCommand = new RelayCommand<Mod>(ExecuteMoveModTop, CanMoveModTop));
        public RelayCommand<Mod> MoveModUpCommand => moveModUpCommand ?? (moveModUpCommand = new RelayCommand<Mod>(ExecuteMoveModUp, CanMoveModUp));
        public RelayCommand<Mod> MoveModDownCommand => moveModDownCommand ?? (moveModDownCommand = new RelayCommand<Mod>(ExecuteMoveModDown, CanMoveModDown));
        public RelayCommand<Mod> MoveModBottomCommand => moveModBottomCommand ?? (moveModBottomCommand = new RelayCommand<Mod>(ExecuteMoveModBottom, CanMoveModBottom));
       
        private ModRegistry modRegistry;
        private GameData gameData;

        private DataCache cache;

        private string currentDirectory;
        private string currentModRegistry;
        private string currentGameData;

        private RelayCommand openModRegistryCommand;
        private RelayCommand openGameDataCommand;
        private RelayCommand saveGameDataCommand;
        private RelayCommand openOptionsCommand;

        private RelayCommand<Mod> moveModTopCommand;
        private RelayCommand<Mod> moveModUpCommand;
        private RelayCommand<Mod> moveModDownCommand;
        private RelayCommand<Mod> moveModBottomCommand;

        private Mod selectedMod;

        public MainWindowViewModel()
        {
            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}Paradox Interactive{Path.DirectorySeparatorChar}Stellaris";
            if (Directory.Exists(path))
            {
                currentDirectory = path;

                if (File.Exists($"{path}{Path.DirectorySeparatorChar}mods_registry.json") &&
                    File.Exists($"{path}{Path.DirectorySeparatorChar}game_data.json"))
                {
                    OpenModRegistry($"{path}{Path.DirectorySeparatorChar}mods_registry.json");
                    OpenGameData($"{path}{Path.DirectorySeparatorChar}game_data.json");
                }
            }
            else
            {
                currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        private void OpenModRegistry(string file)
        {
            if (!File.Exists(file))
            {
                return;
            }

            SelectedMod = null;

            currentDirectory = Path.GetDirectoryName(file);
            currentModRegistry = file;
            modRegistry = JsonConvert.DeserializeObject<ModRegistry>(File.ReadAllText(file));

            UpdateGameData();
            BuildDataCache();

            OnPropertyChanged(nameof(Mods));
        }

        private void OpenGameData(string file)
        {
            if (!File.Exists(file))
            {
                return;
            }

            SelectedMod = null;

            currentDirectory = Path.GetDirectoryName(file);
            currentGameData = file;
            gameData = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(file));

            UpdateGameData();
            BuildDataCache();

            OnPropertyChanged(nameof(Mods));
        }

        private void SaveGameData(string file)
        {
            if (File.Exists(file))
            {
                File.Copy(file, $"{Path.GetDirectoryName(file)}{Path.DirectorySeparatorChar}{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileName(file)}.bak", true);
            }

            currentDirectory = Path.GetDirectoryName(file);
            File.WriteAllText(file, JsonConvert.SerializeObject(gameData));
        }

        private void UpdateGameData()
        {
            if (modRegistry == null)
            {
                return;
            }

            if (gameData == null)
            {
                gameData = new GameData();
            }

            foreach (var mod in modRegistry.Values)
            {
                if (!gameData.ModsOrder.Contains(mod.ID))
                {
                    gameData.ModsOrder.Insert(0, mod.ID);
                }
            }

            BuildDataCache();

            OnPropertyChanged(nameof(Mods));
        }

        private void BuildDataCache()
        {
            if (modRegistry == null)
            {
                return;
            }
            if (gameData == null)
            {
                return;
            }

            if (cache != null)
            {
                cache.CancelCaching();
            }

            Cache = new DataCache(modRegistry, gameData);
        }

        private void ExecuteOpenModRegistry(object obj)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                InitialDirectory = currentDirectory,
                Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (currentModRegistry != null)
            {
                dialog.FileName = currentModRegistry;
            }

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            OpenModRegistry(dialog.FileName);
        }

        private void ExecuteOpenGameData(object obj)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                InitialDirectory = currentDirectory,
                Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (currentGameData != null)
            {
                dialog.FileName = currentGameData;
            }

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            OpenGameData(dialog.FileName);
        }

        private void ExecuteOpenOptions(object obj)
        {
            throw new NotImplementedException();
        }

        private bool CanSaveGameData(object obj)
        {
            return gameData != null;
        }

        private void ExecuteSaveGameData(object obj)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                InitialDirectory = currentDirectory,
                Filter = "JSON files (*.json)|*.json|Text files (*.txt)",
                CreatePrompt = false,
                OverwritePrompt = true,
                AddExtension = true
            };

            if (currentGameData != null)
            {
                dialog.FileName = currentGameData;
            }

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            SaveGameData(dialog.FileName);
        }

        private bool CanMoveMod()
        {
            return modRegistry != null && gameData != null && selectedMod != null && gameData.ModsOrder.Contains(selectedMod.ID);
        }

        private bool CanMoveModTop(Mod obj)
        {
            return CanMoveMod() && selectedMod.ID != gameData.ModsOrder.First();
        }

        private bool CanMoveModUp(Mod obj)
        {
            return CanMoveMod() && selectedMod.ID != gameData.ModsOrder.First();
        }

        private bool CanMoveModDown(Mod obj)
        {
            return CanMoveMod() && selectedMod.ID != gameData.ModsOrder.Last();
        }

        private bool CanMoveModBottom(Mod obj)
        {
            return CanMoveMod() && selectedMod.ID != gameData.ModsOrder.Last();
        }

        private void ExecuteMoveModTop(Mod obj)
        {
            gameData.ModsOrder.Remove(selectedMod.ID);
            gameData.ModsOrder.Insert(0, selectedMod.ID);

            BuildDataCache();

            OnPropertyChanged(nameof(Mods));
        }

        private void ExecuteMoveModUp(Mod obj)
        {
            int index = gameData.ModsOrder.IndexOf(selectedMod.ID);
            gameData.ModsOrder.Remove(selectedMod.ID);
            gameData.ModsOrder.Insert(index - 1, selectedMod.ID);

            BuildDataCache();

            OnPropertyChanged(nameof(Mods));
        }

        private void ExecuteMoveModDown(Mod obj)
        {
            int index = gameData.ModsOrder.IndexOf(selectedMod.ID);
            gameData.ModsOrder.Remove(selectedMod.ID);
            gameData.ModsOrder.Insert(index + 1, selectedMod.ID);

            BuildDataCache();

            OnPropertyChanged(nameof(Mods));
        }

        private void ExecuteMoveModBottom(Mod obj)
        {
            gameData.ModsOrder.Remove(selectedMod.ID);
            gameData.ModsOrder.Add(selectedMod.ID);

            BuildDataCache();

            OnPropertyChanged(nameof(Mods));
        }
    }
}