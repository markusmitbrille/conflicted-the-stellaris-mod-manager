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
        private RelayCommand openModRegistryCommand;
        public RelayCommand OpenModRegistryCommand => openModRegistryCommand ?? (openModRegistryCommand = new RelayCommand(OpenModRegistry));

        private RelayCommand openGameDataCommand;
        public RelayCommand OpenGameDataCommand => openGameDataCommand ?? (openGameDataCommand = new RelayCommand(OpenGameData));

        private RelayCommand saveGameDataCommand;
        public RelayCommand SaveGameDataCommand => saveGameDataCommand ?? (saveGameDataCommand = new RelayCommand(SaveGameData, CanSaveGameData));

        private RelayCommand openOptionsCommand;
        public RelayCommand OpenOptionsCommand => openOptionsCommand ?? (openOptionsCommand = new RelayCommand(OpenOptions));

        private RelayCommand<Mod> moveModTopCommand;
        public RelayCommand<Mod> MoveModTopCommand => moveModTopCommand ?? (moveModTopCommand = new RelayCommand<Mod>(MoveModTop, CanMoveModTop));

        private RelayCommand<Mod> moveModUpCommand;
        public RelayCommand<Mod> MoveModUpCommand => moveModUpCommand ?? (moveModUpCommand = new RelayCommand<Mod>(MoveModUp, CanMoveModUp));

        private RelayCommand<Mod> moveModDownCommand;
        public RelayCommand<Mod> MoveModDownCommand => moveModDownCommand ?? (moveModDownCommand = new RelayCommand<Mod>(MoveModDown, CanMoveModDown));

        private RelayCommand<Mod> moveModBottomCommand;
        public RelayCommand<Mod> MoveModBottomCommand => moveModBottomCommand ?? (moveModBottomCommand = new RelayCommand<Mod>(MoveModBottom, CanMoveModBottom));

        public IEnumerable<Mod> Mods => gameData == null ? modRegistry?.Values.OrderBy(mod => mod.DisplayName) : modRegistry?.Values.OrderBy(mod => mod, gameData);

        private Mod selectedMod;

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

        public int CountFilesOverwrittenFrom => throw new NotImplementedException();
        public int CountFileConflictsCreated => throw new NotImplementedException();
        public IEnumerable<Conflict> FileConflictsCreated => throw new NotImplementedException();

        private Conflict selectedCreatedFileConflict;
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

        public int CountFilesOverwrittenBy => throw new NotImplementedException();
        public int CountFileConflictsSuffered => throw new NotImplementedException();
        public IEnumerable<Conflict> FileConflictsSuffered => throw new NotImplementedException();

        private Conflict selectedSufferedFileConflict;
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

        public int CountElementsOverwrittenFrom => throw new NotImplementedException();
        public int CountElementConflictsCreated => throw new NotImplementedException();
        public IEnumerable<Conflict> ElementConflictsCreated => throw new NotImplementedException();

        private Conflict selectedCreatedElementConflict;
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

        public int CountElementsOverwrittenBy => throw new NotImplementedException();
        public int CountElementConflictsSuffered => throw new NotImplementedException();
        public IEnumerable<Conflict> ElementConflictsSuffered => throw new NotImplementedException();

        private Conflict selectedSufferedElementConflict;
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

        private ModRegistry modRegistry;
        private GameData gameData;

        private string currentDirectory;

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

        private bool CanSaveGameData(object obj)
        {
            return gameData != null;
        }

        private void OpenOptions(object obj)
        {
            throw new NotImplementedException();
        }

        private void MoveModTop(Mod obj)
        {
            gameData.ModsOrder.Remove(selectedMod.ID);
            gameData.ModsOrder.Insert(0, selectedMod.ID);

            OnPropertyChanged(nameof(Mods));
        }

        private bool CanMoveModTop(Mod obj)
        {
            return CanMoveMod() && selectedMod.ID != gameData.ModsOrder.First();
        }

        private void MoveModUp(Mod obj)
        {
            int index = gameData.ModsOrder.IndexOf(selectedMod.ID);
            gameData.ModsOrder.Remove(selectedMod.ID);
            gameData.ModsOrder.Insert(index - 1, selectedMod.ID);

            OnPropertyChanged(nameof(Mods));
        }

        private bool CanMoveModUp(Mod obj)
        {
            return CanMoveMod() && selectedMod.ID != gameData.ModsOrder.First();
        }

        private void MoveModDown(Mod obj)
        {
            int index = gameData.ModsOrder.IndexOf(selectedMod.ID);
            gameData.ModsOrder.Remove(selectedMod.ID);
            gameData.ModsOrder.Insert(index + 1, selectedMod.ID);

            OnPropertyChanged(nameof(Mods));
        }

        private bool CanMoveModDown(Mod obj)
        {
            return CanMoveMod() && selectedMod.ID != gameData.ModsOrder.Last();
        }

        private void MoveModBottom(Mod obj)
        {
            gameData.ModsOrder.Remove(selectedMod.ID);
            gameData.ModsOrder.Add(selectedMod.ID);

            OnPropertyChanged(nameof(Mods));
        }

        private bool CanMoveModBottom(Mod obj)
        {
            return CanMoveMod() && selectedMod.ID != gameData.ModsOrder.Last();
        }

        private bool CanMoveMod()
        {
            return modRegistry != null && gameData != null && selectedMod != null && gameData.ModsOrder.Contains(selectedMod.ID);
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
    }
}