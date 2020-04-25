using Conflicted.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Conflicted.ViewModel
{
    internal class ModlistViewModel : ModelViewModel<Modlist>
    {
        private static readonly Dictionary<Modlist, ModlistViewModel> instances = new Dictionary<Modlist, ModlistViewModel>();

        private IEnumerable<ModViewModel> mods;
        public IEnumerable<ModViewModel> Mods => mods ?? (mods = model.Mods.Select(file => ModViewModel.Create(file)).ToArray());

        private int? modCount;
        public int? ModCount => modCount ?? (modCount = Mods.Count());

        private string progressBarLabel;
        public string ProgressBarLabel
        {
            get => progressBarLabel;
            set
            {
                progressBarLabel = value;
                OnPropertyChanged();
            }
        }

        private bool progressBarIsIndeterminate;
        public bool ProgressBarIsIndeterminate
        {
            get => progressBarIsIndeterminate;
            set
            {
                progressBarIsIndeterminate = value;
                OnPropertyChanged();
            }
        }

        private int progress = 0;
        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged();
            }
        }

        private int goal = 0;
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

        private Task cachingTask;
        private CancellationTokenSource cachingCancellation;

        private ModlistViewModel(Modlist model) : base(model)
        {
            instances[model] = this;

            model.RegistryLoading += Model_RegistryLoading;
            model.RegistryLoaded += Model_RegistryLoaded;

            model.DataLoading += Model_DataLoading;
            model.DataLoaded += Model_DataLoaded;

            model.ModMovedTop += Model_ModMovedTop;
            model.ModMovedUp += Model_ModMovedUp;
            model.ModMovedDown += Model_ModMovedDown;
            model.ModMovedBottom += Model_ModMovedBottom;

            TryOpenDefaultFiles();
        }

        private void StartCaching()
        {
            if (!cachingTask?.IsCompleted ?? false)
            {
                cachingCancellation.Cancel();
                cachingTask.Wait();
            }

            cachingCancellation = new CancellationTokenSource();
            cachingTask = Task.Run(Cache, cachingCancellation.Token);
        }

        public static ModlistViewModel Create(Modlist model)
        {
            if (instances.ContainsKey(model))
            {
                return instances[model];
            }

            return new ModlistViewModel(model);
        }

        public static void Flush()
        {
            instances.Clear();
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

        private void Cache()
        {
            if (cachingCancellation?.IsCancellationRequested ?? true)
            {
                Progress = 0;
                Goal = 0;
                return;
            }

            ProgressBarIsIndeterminate = true;
            ProgressBarLabel = "Building File Conflict Cache";

            model.FileConflicts.ToArray();

            ProgressBarLabel = "Building Element Conflict Cache";

            model.ElementConflicts.ToArray();

            Progress = 0;
            Goal = model.Mods.Count();
            ProgressBarIsIndeterminate = false;

            foreach (var mod in Mods)
            {
                if (cachingCancellation?.IsCancellationRequested ?? true)
                {
                    Progress = 0;
                    Goal = 0;
                    return;
                }

                ProgressBarLabel = $"Loading Mod {Progress + 1}/{Goal}";

                mod.Page.ToString();

                Progress++;
            }

            ProgressBarIsIndeterminate = true;
            ProgressBarLabel = "";
            Progress = 0;
            Goal = model.Mods.SelectMany(mod => mod.Files).Count();
            ProgressBarIsIndeterminate = false;

            foreach (var file in Mods.SelectMany(mod => mod.Files))
            {
                if (cachingCancellation?.IsCancellationRequested ?? true)
                {
                    Progress = 0;
                    Goal = 0;
                    return;
                }

                ProgressBarLabel = $"Loading File {Progress + 1}/{Goal}";
                Progress++;
            }

            ProgressBarIsIndeterminate = true;
            ProgressBarLabel = "";
            Progress = 0;
            Goal = model.Mods.SelectMany(mod => mod.Elements).Count();
            ProgressBarIsIndeterminate = false;

            foreach (var element in Mods.SelectMany(mod => mod.Elements))
            {
                if (cachingCancellation?.IsCancellationRequested ?? true)
                {
                    Progress = 0;
                    Goal = 0;
                    return;
                }

                ProgressBarLabel = $"Loading Element {Progress + 1}/{Goal}";
                Progress++;
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

        private void Model_RegistryLoading(object sender, EventArgs e)
        {
            cachingCancellation?.Cancel();
        }

        private void Model_RegistryLoaded(object sender, EventArgs e)
        {
            ModViewModel.Flush();
            ModFileViewModel.Flush();
            ModElementViewModel.Flush();

            mods = null;
            OnPropertyChanged(nameof(Mods));

            StartCaching();
        }

        private void Model_DataLoading(object sender, EventArgs e)
        {
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