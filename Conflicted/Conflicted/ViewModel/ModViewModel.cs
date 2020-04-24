using Conflicted.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows.Media;

namespace Conflicted.ViewModel
{
    internal class ModViewModel : ModelViewModel<Mod>
    {
        private static readonly Dictionary<Mod, ModViewModel> instances = new Dictionary<Mod, ModViewModel>();

        public long? SteamID => model.SteamID;
        public string DisplayName => model.DisplayName;
        public ReadOnlyCollection<string> Tags => model.Tags;
        public long? TimeUpdated => model.TimeUpdated;
        public string Source => model.Source;
        public string ThumbnailUrl => model.ThumbnailUrl;
        public string DirPath => model.DirPath;
        public string Status => model.Status;
        public string ID => model.ID;
        public string GameRegistryId => model.GameRegistryId;
        public string RequiredVersion => model.RequiredVersion;
        public string ArchivePath => model.ArchivePath;
        public string Cause => model.Cause;
        public string ThumbnailPath => model.ThumbnailPath;
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

        private ModlistViewModel modlist;
        public ModlistViewModel Modlist => modlist ?? (modlist = ModlistViewModel.Create(model.Modlist));

        private IEnumerable<ModFileViewModel> files;
        public IEnumerable<ModFileViewModel> Files
        {
            get
            {
                return files ?? (files = model.Files
                    .Select(file => ModFileViewModel.Create(file))
                    .OrderByDescending(file => file.ConflictCount)
                    .ThenBy(file => file.ID)
                    .ToArray());
            }
        }

        private int? fileCount;
        public int? FileCount => fileCount ?? (fileCount = Files.Count());

        private int? fileConflictCount;
        public int? FileConflictCount
        {
            get
            {
                return fileConflictCount ?? (fileConflictCount = model.Modlist.FileConflicts
                    .SelectMany(group => group)
                    .Where(file => file.Mod == model)
                    .Count());
            }
        }

        private IEnumerable<ModElementViewModel> elements;
        public IEnumerable<ModElementViewModel> Elements
        {
            get
            {
                return elements ?? (elements = model.Elements
                    .Select(element => ModElementViewModel.Create(element))
                    .OrderByDescending(element => element.ConflictCount)
                    .ThenBy(element => element.File.ID)
                    .ThenBy(element => element.ID)
                    .ToArray());
            }
        }

        private int? elementConflictCount;
        public int? ElementConflictCount
        {
            get
            {
                return elementConflictCount ?? (elementConflictCount = model.Modlist.ElementConflicts
                    .SelectMany(group => group)
                    .Where(element => element.Mod == model)
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
        public RelayCommand MoveTopCommand => moveTopCommand ?? (moveTopCommand = new RelayCommand(ExecuteMoveTop));

        private RelayCommand moveUpCommand;
        public RelayCommand MoveUpCommand => moveUpCommand ?? (moveUpCommand = new RelayCommand(ExecuteMoveUp));

        private RelayCommand moveDownCommand;
        public RelayCommand MoveDownCommand => moveDownCommand ?? (moveDownCommand = new RelayCommand(ExecuteMoveDown));

        private RelayCommand moveBottomCommand;
        public RelayCommand MoveBottomCommand => moveBottomCommand ?? (moveBottomCommand = new RelayCommand(ExecuteMoveBottom));

        private ModViewModel(Mod model) : base(model)
        {
            instances[model] = this;
        }

        public static ModViewModel Create(Mod model)
        {
            if (instances.ContainsKey(model))
            {
                return instances[model];
            }

            return new ModViewModel(model);
        }

        private void ExecuteMoveTop(object obj) => model.MoveTop();

        private void ExecuteMoveUp(object obj) => model.MoveUp();

        private void ExecuteMoveDown(object obj) => model.MoveDown();

        private void ExecuteMoveBottom(object obj) => model.MoveBottom();

        public class OrderComparer : Comparer<ModViewModel>
        {
            public static OrderComparer Instance => instance ?? (instance = new OrderComparer());
            private static OrderComparer instance;

            private OrderComparer()
            {
            }

            public override int Compare(ModViewModel x, ModViewModel y)
            {
                return Mod.OrderComparer.Instance.Compare(x.model, y.model);
            }
        }
    }
}