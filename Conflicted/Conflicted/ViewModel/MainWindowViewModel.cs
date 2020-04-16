using Conflicted.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Conflicted.ViewModel
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private ModRegistry modRegistry;
        private GameData gameData;

        private string currentDirectory;

        private RelayCommand openModRegistryCommand;
        private RelayCommand openGameDataCommand;
        private RelayCommand saveGameDataCommand;
        private RelayCommand openOptionsCommand;

        private RelayCommand<Mod> moveModTopCommand;
        private RelayCommand<Mod> moveModUpCommand;
        private RelayCommand<Mod> moveModDownCommand;
        private RelayCommand<Mod> moveModBottomCommand;

        private Mod selectedMod;

        private Conflict selectedCreatedFileConflict;
        private Conflict selectedSufferedFileConflict;
        private Conflict selectedCreatedElementConflict;
        private Conflict selectedSufferedElementConflict;

        public IEnumerable<Mod> Mods => gameData == null ? modRegistry?.Values.OrderBy(mod => mod.DisplayName) : modRegistry?.Values.OrderBy(mod => mod, gameData);

        public IEnumerable<Conflict> FileConflictsCreated
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

                return
                    from conflict in modRegistry.FileConflicts
                    where conflict.Mod != selectedMod
                    where conflict.Conflictors.Contains(selectedMod)
                    where gameData.ModsOrder.IndexOf(conflict.Mod.ID) < gameData.ModsOrder.IndexOf(selectedMod.ID)
                    select conflict;
            }
        }

        public IEnumerable<Conflict> FileConflictsSuffered
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

                return
                    from conflict in modRegistry.FileConflicts
                    where conflict.Mod != selectedMod
                    where conflict.Conflictors.Contains(selectedMod)
                    where gameData.ModsOrder.IndexOf(conflict.Mod.ID) > gameData.ModsOrder.IndexOf(selectedMod.ID)
                    select conflict;
            }
        }

        public IEnumerable<Conflict> ElementConflictsCreated
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

                return
                    from conflict in modRegistry.ElementConflicts
                    where conflict.Mod != selectedMod
                    where conflict.Conflictors.Contains(selectedMod)
                    where gameData.ModsOrder.IndexOf(conflict.Mod.ID) < gameData.ModsOrder.IndexOf(selectedMod.ID)
                    select conflict;
            }
        }

        public IEnumerable<Conflict> ElementConflictsSuffered
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

                return
                    from conflict in modRegistry.ElementConflicts
                    where conflict.Mod != selectedMod
                    where conflict.Conflictors.Contains(selectedMod)
                    where gameData.ModsOrder.IndexOf(conflict.Mod.ID) > gameData.ModsOrder.IndexOf(selectedMod.ID)
                    select conflict;
            }
        }

        public int CountElementConflictsCreated => ElementConflictsCreated.Count();
        public int CountElementsOverwrittenFrom => ElementConflictsCreated.SelectMany(conflict => conflict.Conflictors).Distinct().Count();

        public int CountElementConflictsSuffered => ElementConflictsSuffered.Count();
        public int CountElementsOverwrittenBy => ElementConflictsSuffered.SelectMany(conflict => conflict.Conflictors).Distinct().Count();

        public int CountFileConflictsCreated => FileConflictsCreated.Count();
        public int CountFilesOverwrittenFrom => FileConflictsCreated.SelectMany(conflict => conflict.Conflictors).Distinct().Count();

        public int CountFileConflictsSuffered => FileConflictsSuffered.Count();
        public int CountFilesOverwrittenBy => FileConflictsSuffered.SelectMany(conflict => conflict.Conflictors).Distinct().Count();

        public Mod SelectedMod
        {
            get => selectedMod;
            set
            {
                selectedMod = value;
                SelectedCreatedFileConflict = null;
                SelectedSufferedFileConflict = null;
                SelectedCreatedElementConflict = null;
                SelectedSufferedElementConflict = null;

                OnPropertyChanged();
                OnPropertyChanged(nameof(CountFilesOverwrittenFrom));
                OnPropertyChanged(nameof(CountFileConflictsCreated));
                OnPropertyChanged(nameof(FileConflictsCreated));
                OnPropertyChanged(nameof(CountFilesOverwrittenBy));
                OnPropertyChanged(nameof(CountFileConflictsSuffered));
                OnPropertyChanged(nameof(FileConflictsSuffered));
                OnPropertyChanged(nameof(CountElementsOverwrittenFrom));
                OnPropertyChanged(nameof(CountElementConflictsCreated));
                OnPropertyChanged(nameof(ElementConflictsCreated));
                OnPropertyChanged(nameof(CountElementsOverwrittenBy));
                OnPropertyChanged(nameof(CountElementConflictsSuffered));
                OnPropertyChanged(nameof(ElementConflictsSuffered));
            }
        }

        public Conflict SelectedCreatedFileConflict
        {
            get => selectedCreatedFileConflict;
            set
            {
                if (value != null)
                {
                    SelectedSufferedFileConflict = null;
                    SelectedCreatedElementConflict = null;
                    SelectedSufferedElementConflict = null;
                }

                selectedCreatedFileConflict = value;
                OnPropertyChanged();
            }
        }

        public Conflict SelectedSufferedFileConflict
        {
            get => selectedSufferedFileConflict;
            set
            {
                if (value != null)
                {
                    SelectedCreatedFileConflict = null;
                    SelectedCreatedElementConflict = null;
                    SelectedSufferedElementConflict = null;
                }

                selectedSufferedFileConflict = value;
                OnPropertyChanged();
            }
        }

        public Conflict SelectedCreatedElementConflict
        {
            get => selectedCreatedElementConflict;
            set
            {
                if (value != null)
                {
                    SelectedCreatedFileConflict = null;
                    SelectedSufferedFileConflict = null;
                    SelectedSufferedElementConflict = null;
                }

                selectedCreatedElementConflict = value;
                OnPropertyChanged();
            }
        }

        public Conflict SelectedSufferedElementConflict
        {
            get => selectedSufferedElementConflict;
            set
            {
                if (value != null)
                {
                    SelectedCreatedFileConflict = null;
                    SelectedSufferedFileConflict = null;
                    SelectedCreatedElementConflict = null;
                }

                selectedSufferedElementConflict = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand OpenModRegistryCommand => openModRegistryCommand ?? (openModRegistryCommand = new RelayCommand(OpenModRegistry));
        public RelayCommand OpenGameDataCommand => openGameDataCommand ?? (openGameDataCommand = new RelayCommand(OpenGameData));
        public RelayCommand SaveGameDataCommand => saveGameDataCommand ?? (saveGameDataCommand = new RelayCommand(SaveGameData, CanSaveGameData));
        public RelayCommand OpenOptionsCommand => openOptionsCommand ?? (openOptionsCommand = new RelayCommand(OpenOptions));

        public RelayCommand<Mod> MoveModTopCommand => moveModTopCommand ?? (moveModTopCommand = new RelayCommand<Mod>(MoveModTop, CanMoveModTop));
        public RelayCommand<Mod> MoveModUpCommand => moveModUpCommand ?? (moveModUpCommand = new RelayCommand<Mod>(MoveModUp, CanMoveModUp));
        public RelayCommand<Mod> MoveModDownCommand => moveModDownCommand ?? (moveModDownCommand = new RelayCommand<Mod>(MoveModDown, CanMoveModDown));
        public RelayCommand<Mod> MoveModBottomCommand => moveModBottomCommand ?? (moveModBottomCommand = new RelayCommand<Mod>(MoveModBottom, CanMoveModBottom));

        public MainWindowViewModel()
        {
        }

        private void OpenModRegistry(object obj)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                InitialDirectory = currentDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            if (!File.Exists(dialog.FileName))
            {
                return;
            }

            SelectedMod = null;

            currentDirectory = Path.GetDirectoryName(dialog.FileName);
            modRegistry = JsonConvert.DeserializeObject<ModRegistry>(File.ReadAllText(dialog.FileName));

            UpdateGameData();

            OnPropertyChanged(nameof(Mods));
        }

        private void OpenGameData(object obj)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                InitialDirectory = currentDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            if (!File.Exists(dialog.FileName))
            {
                return;
            }

            SelectedMod = null;

            currentDirectory = Path.GetDirectoryName(dialog.FileName);
            gameData = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(dialog.FileName));

            UpdateGameData();

            OnPropertyChanged(nameof(Mods));
        }

        private void OpenOptions(object obj)
        {
            throw new NotImplementedException();
        }

        private bool CanSaveGameData(object obj)
        {
            return gameData != null;
        }

        private void SaveGameData(object obj)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                InitialDirectory = currentDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "JSON files (*.json)|*.json|Text files (*.txt)",
                CreatePrompt = false,
                OverwritePrompt = true,
                AddExtension = true
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            currentDirectory = Path.GetDirectoryName(dialog.FileName);
            File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(gameData));
        }

        private void UpdateGameData()
        {
            if (modRegistry == null)
            {
                return;
            }

            if (gameData == null)
            {
                return;
            }

            foreach (var mod in modRegistry.Values)
            {
                if (!gameData.ModsOrder.Contains(mod.ID))
                {
                    gameData.ModsOrder.Insert(0, mod.ID);
                }
            }

            OnPropertyChanged(nameof(Mods));
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

        private void MoveModTop(Mod obj)
        {
            gameData.ModsOrder.Remove(selectedMod.ID);
            gameData.ModsOrder.Insert(0, selectedMod.ID);

            OnPropertyChanged(nameof(Mods));
        }

        private void MoveModUp(Mod obj)
        {
            int index = gameData.ModsOrder.IndexOf(selectedMod.ID);
            gameData.ModsOrder.Remove(selectedMod.ID);
            gameData.ModsOrder.Insert(index - 1, selectedMod.ID);

            OnPropertyChanged(nameof(Mods));
        }

        private void MoveModDown(Mod obj)
        {
            int index = gameData.ModsOrder.IndexOf(selectedMod.ID);
            gameData.ModsOrder.Remove(selectedMod.ID);
            gameData.ModsOrder.Insert(index + 1, selectedMod.ID);

            OnPropertyChanged(nameof(Mods));
        }

        private void MoveModBottom(Mod obj)
        {
            gameData.ModsOrder.Remove(selectedMod.ID);
            gameData.ModsOrder.Add(selectedMod.ID);

            OnPropertyChanged(nameof(Mods));
        }
    }
}