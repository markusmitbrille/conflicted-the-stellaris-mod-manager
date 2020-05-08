using Conflicted.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows.Media;

namespace Conflicted.ViewModel
{
    class ModViewModel : ModelViewModel<Mod>
    {
        private static readonly Dictionary<Mod, ModViewModel> instances = new Dictionary<Mod, ModViewModel>();

        public long? SteamID => Model.SteamID;
        public string DisplayName => Model.DisplayName;
        public ReadOnlyCollection<string> Tags => Model.Tags;
        public long? TimeUpdated => Model.TimeUpdated;
        public string Source => Model.Source;
        public string ThumbnailUrl => Model.ThumbnailUrl;
        public string DirPath => Model.DirPath;
        public string Status => Model.Status;
        public string ID => Model.ID;
        public string GameRegistryId => Model.GameRegistryId;
        public string RequiredVersion => Model.RequiredVersion;
        public string ArchivePath => Model.ArchivePath;
        public string Cause => Model.Cause;
        public string ThumbnailPath => Model.ThumbnailPath;
        public string WebPageUrl => SteamID.HasValue ? $"https://steamcommunity.com/sharedfiles/filedetails/?id={SteamID.Value}" : "https://www.google.com";

        private string page;
        public string Page
        {
            get
            {
                if (page == null)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        page = webClient.DownloadString(WebPageUrl);
                    }
                }

                return page;
            }
        }

        private ModListViewModel modlist;
        public ModListViewModel Modlist => modlist ?? (modlist = ModListViewModel.Create(Model.Modlist));

        private IEnumerable<ModFileViewModel> files;
        public IEnumerable<ModFileViewModel> Files
        {
            get
            {
                return files ?? (files = Model.Files
                    .Select(file => ModFileViewModel.Create(file))
                    .OrderByDescending(file => file.ConflictCount)
                    .ThenBy(file => file.ID)
                    .ToList()
                    .AsReadOnly());
            }
        }

        private int? fileCount;
        public int? FileCount => fileCount ?? (fileCount = Files.Count());

        private int? fileConflictCount;
        public int? FileConflictCount
        {
            get
            {
                return fileConflictCount ?? (fileConflictCount = Model.Modlist.FileConflicts
                    .SelectMany(group => group)
                    .Where(file => file.Mod == Model)
                    .Count());
            }
        }

        private IEnumerable<ModElementViewModel> elements;
        public IEnumerable<ModElementViewModel> Elements
        {
            get
            {
                return elements ?? (elements = Model.Elements
                    .Select(element => ModElementViewModel.Create(element))
                    .OrderByDescending(element => element.ConflictCount)
                    .ThenBy(element => element.File.ID)
                    .ThenBy(element => element.ID)
                    .ToList()
                    .AsReadOnly());
            }
        }

        private int? elementConflictCount;
        public int? ElementConflictCount
        {
            get
            {
                return elementConflictCount ?? (elementConflictCount = Model.Modlist.ElementConflicts
                    .SelectMany(group => group)
                    .Where(element => element.Mod == Model)
                    .Count());
            }
        }

        private int? elementCount;
        public int? ElementCount => elementCount ?? (elementCount = Elements.Count());

        private int? conflictCount;
        public int? ConflictCount => conflictCount ?? (conflictCount = FileConflictCount + ElementConflictCount);

        private Brush conflictCountBrush;
        public Brush ConflictCountBrush => conflictCountBrush ?? (conflictCountBrush = ConflictCount > 0 ? Brushes.Red : Brushes.Black);

        private RelayCommand moveTopCommand;
        public RelayCommand MoveTopCommand => moveTopCommand ?? (moveTopCommand = new RelayCommand(ExecuteMoveTop, CanExecuteMoveTop));

        private RelayCommand moveUpCommand;
        public RelayCommand MoveUpCommand => moveUpCommand ?? (moveUpCommand = new RelayCommand(ExecuteMoveUp, CanExecuteMoveUp));

        private RelayCommand moveDownCommand;
        public RelayCommand MoveDownCommand => moveDownCommand ?? (moveDownCommand = new RelayCommand(ExecuteMoveDown, CanExecuteMoveDown));

        private RelayCommand moveBottomCommand;
        public RelayCommand MoveBottomCommand => moveBottomCommand ?? (moveBottomCommand = new RelayCommand(ExecuteMoveBottom, CanExecuteMoveBottom));

        private Brush backgroundBrush = Brushes.Transparent;
        public Brush BackgroundBrush
        {
            get => backgroundBrush;
            set
            {
                backgroundBrush = value;
                OnPropertyChanged();
            }
        }

        private Brush foregroundBrush = Brushes.Black;
        public Brush ForegroundBrush
        {
            get => foregroundBrush;
            set
            {
                foregroundBrush = value;
                OnPropertyChanged();
            }
        }

        private ModViewModel(Mod model) : base(model)
        {
            instances[model] = this;

            model.Modlist.RegistryLoaded += Modlist_RegistryLoaded;
        }

        private void Modlist_RegistryLoaded(object sender, System.EventArgs e)
        {
            instances.Remove(Model);

            Model.Modlist.RegistryLoaded -= Modlist_RegistryLoaded;
        }

        public static ModViewModel Create(Mod model)
        {
            if (instances.ContainsKey(model))
            {
                return instances[model];
            }

            return new ModViewModel(model);
        }

        private bool CanExecuteMoveTop(object obj) => Model.Modlist.Order.First() != Model.ID;

        private bool CanExecuteMoveUp(object obj) => Model.Modlist.Order.First() != Model.ID;

        private bool CanExecuteMoveDown(object obj) => Model.Modlist.Order.Last() != Model.ID;

        private bool CanExecuteMoveBottom(object obj) => Model.Modlist.Order.Last() != Model.ID;

        private void ExecuteMoveTop(object obj) => Model.MoveTop();

        private void ExecuteMoveUp(object obj) => Model.MoveUp();

        private void ExecuteMoveDown(object obj) => Model.MoveDown();

        private void ExecuteMoveBottom(object obj) => Model.MoveBottom();

        public class OrderComparer : Comparer<ModViewModel>
        {
            public static OrderComparer Instance => instance ?? (instance = new OrderComparer());
            private static OrderComparer instance;

            private OrderComparer()
            {
            }

            public override int Compare(ModViewModel x, ModViewModel y)
            {
                return Mod.OrderComparer.Instance.Compare(x.Model, y.Model);
            }
        }
    }
}