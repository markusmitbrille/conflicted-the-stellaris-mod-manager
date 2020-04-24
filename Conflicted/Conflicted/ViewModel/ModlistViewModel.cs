using Conflicted.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Conflicted.ViewModel
{
    internal class ModlistViewModel : ModelViewModel<Modlist>
    {
        private static readonly Dictionary<Modlist, ModlistViewModel> instances = new Dictionary<Modlist, ModlistViewModel>();

        private IEnumerable<ModViewModel> mods;
        public IEnumerable<ModViewModel> Mods => mods ?? (mods = model.Mods.Select(file => ModViewModel.Create(file)).ToArray());

        private int? modCount;
        public int? ModCount => modCount ?? (modCount = Mods.Count());

        private int progress;
        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged();
            }
        }

        private int goal;
        public int Goal
        {
            get => goal;
            set
            {
                goal = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand openModRegistryCommand;
        public RelayCommand OpenModRegistryCommand => openModRegistryCommand ?? (openModRegistryCommand = new RelayCommand(ExecuteOpenModRegistry));

        private RelayCommand openGameDataCommand;
        public RelayCommand OpenGameDataCommand => openGameDataCommand ?? (openGameDataCommand = new RelayCommand(ExecuteOpenGameData));

        private RelayCommand saveGameDataCommand;
        public RelayCommand SaveGameDataCommand => saveGameDataCommand ?? (saveGameDataCommand = new RelayCommand(ExecuteSaveGameData));

        private string currentDirectory;
        private string currentModRegistry;
        private string currentGameData;

        private ModlistViewModel(Modlist model) : base(model)
        {
            instances[model] = this;

            model.RegistryLoaded += Model_RegistryLoaded;
            model.DataLoaded += Model_DataLoaded;

            model.ModMovedTop += Model_ModMovedTop;
            model.ModMovedUp += Model_ModMovedUp;
            model.ModMovedDown += Model_ModMovedDown;
            model.ModMovedBottom += Model_ModMovedBottom;

            TryOpenDefaultFiles();
        }

        public static ModlistViewModel Create(Modlist model)
        {
            if (instances.ContainsKey(model))
            {
                return instances[model];
            }

            return new ModlistViewModel(model);
        }

        private void TryOpenDefaultFiles()
        {
            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}{Path.DirectorySeparatorChar}Paradox Interactive{Path.DirectorySeparatorChar}Stellaris";
            if (Directory.Exists(path))
            {
                currentDirectory = path;

                if (File.Exists($"{path}{Path.DirectorySeparatorChar}mods_registry.json") &&
                    File.Exists($"{path}{Path.DirectorySeparatorChar}game_data.json"))
                {
                    model.OpenModRegistry($"{path}{Path.DirectorySeparatorChar}mods_registry.json");
                    model.OpenGameData($"{path}{Path.DirectorySeparatorChar}game_data.json");
                }
            }
            else
            {
                currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
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

            if (!File.Exists(dialog.FileName))
            {
                return;
            }

            currentDirectory = Path.GetDirectoryName(dialog.FileName);
            currentModRegistry = dialog.FileName;

            model.OpenModRegistry(dialog.FileName);

            OnPropertyChanged(nameof(Mods));
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

            if (!File.Exists(dialog.FileName))
            {
                return;
            }

            currentDirectory = Path.GetDirectoryName(dialog.FileName);
            currentGameData = dialog.FileName;

            model.OpenGameData(dialog.FileName);

            OnPropertyChanged(nameof(Mods));
        }

        private void ExecuteSaveGameData(object obj)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                InitialDirectory = currentDirectory,
                Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt",
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

            currentDirectory = Path.GetDirectoryName(dialog.FileName);

            model.SaveGameData(dialog.FileName);
        }

        private void Model_RegistryLoaded(object sender, EventArgs e)
        {
            mods = null;
            OnPropertyChanged(nameof(Mods));
        }

        private void Model_DataLoaded(object sender, EventArgs e)
        {
            mods = null;
            OnPropertyChanged(nameof(Mods));
        }

        private void Model_ModMovedTop(object sender, Modlist.ModMovedEventArgs e)
        {
            mods = null;
            OnPropertyChanged(nameof(Mods));
        }

        private void Model_ModMovedUp(object sender, Modlist.ModMovedEventArgs e)
        {
            mods = null;
            OnPropertyChanged(nameof(Mods));
        }

        private void Model_ModMovedDown(object sender, Modlist.ModMovedEventArgs e)
        {
            mods = null;
            OnPropertyChanged(nameof(Mods));
        }

        private void Model_ModMovedBottom(object sender, Modlist.ModMovedEventArgs e)
        {
            mods = null;
            OnPropertyChanged(nameof(Mods));
        }
    }
}