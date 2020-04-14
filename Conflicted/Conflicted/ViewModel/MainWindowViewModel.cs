using Conflicted.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conflicted.ViewModel
{
    class MainWindowViewModel : BaseViewModel
    {
        private RelayCommand openModRegistryCommand;
        public RelayCommand OpenModRegistryCommand => openModRegistryCommand ?? (openModRegistryCommand = new RelayCommand(OpenModRegistry));

        private RelayCommand openGameDataCommand;
        public RelayCommand OpenGameDataCommand => openGameDataCommand ?? (openGameDataCommand = new RelayCommand(OpenGameData));

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

        public IEnumerable<Mod> Mods => modRegistry.Values.OrderBy(mod => mod, gameData);

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
                OnPropertyChanged(nameof(CountFilesOverwritten));
                OnPropertyChanged(nameof(CountFileConflictsCreated));
                OnPropertyChanged(nameof(FileConflictsCreated));
                OnPropertyChanged(nameof(CountFilesOverwrittenBy));
                OnPropertyChanged(nameof(CountFileConflictsSuffered));
                OnPropertyChanged(nameof(FileConflictsSuffered));
                OnPropertyChanged(nameof(CountElementsOverwritten));
                OnPropertyChanged(nameof(CountElementConflictsCreated));
                OnPropertyChanged(nameof(ElementConflictsCreated));
                OnPropertyChanged(nameof(CountElementsOverwrittenBy));
                OnPropertyChanged(nameof(CountElementConflictsSuffered));
                OnPropertyChanged(nameof(ElementConflictsSuffered));
            }
        }

        public int CountFilesOverwritten => throw new NotImplementedException();
        public int CountFileConflictsCreated => throw new NotImplementedException();
        public IEnumerable<FileConflict> FileConflictsCreated => throw new NotImplementedException();

        private FileConflict selectedCreatedFileConflict;
        public FileConflict SelectedCreatedFileConflict
        {
            get => selectedCreatedFileConflict;
            set
            {
                selectedCreatedFileConflict = value;
                OnPropertyChanged();
            }
        }

        public int CountFilesOverwrittenBy => throw new NotImplementedException();
        public int CountFileConflictsSuffered => throw new NotImplementedException();
        public IEnumerable<FileConflict> FileConflictsSuffered => throw new NotImplementedException();

        private FileConflict selectedSufferedFileConflict;
        public FileConflict SelectedSufferedFileConflict
        {
            get => selectedSufferedFileConflict;
            set
            {
                selectedSufferedFileConflict = value;
                OnPropertyChanged();
            }
        }

        public int CountElementsOverwritten => throw new NotImplementedException();
        public int CountElementConflictsCreated => throw new NotImplementedException();
        public IEnumerable<ElementConflict> ElementConflictsCreated => throw new NotImplementedException();

        private ElementConflict selectedCreatedElementConflict;
        public ElementConflict SelectedCreatedElementConflict
        {
            get => selectedCreatedElementConflict;
            set
            {
                selectedCreatedElementConflict = value;
                OnPropertyChanged();
            }
        }

        public int CountElementsOverwrittenBy => throw new NotImplementedException();
        public int CountElementConflictsSuffered => throw new NotImplementedException();
        public IEnumerable<ElementConflict> ElementConflictsSuffered => throw new NotImplementedException();

        private ElementConflict selectedSufferedElementConflict;
        public ElementConflict SelectedSufferedElementConflict
        {
            get => selectedSufferedElementConflict;
            set
            {
                selectedSufferedElementConflict = value;
                OnPropertyChanged();
            }
        }

        private ModRegistry modRegistry;
        private GameData gameData;

        public MainWindowViewModel()
        {
        }

        private void OpenModRegistry(object obj)
        {
            throw new NotImplementedException();
        }

        private void OpenGameData(object obj)
        {
            throw new NotImplementedException();
        }

        private void OpenOptions(object obj)
        {
            throw new NotImplementedException();
        }

        private void MoveModTop(Mod obj)
        {
            throw new NotImplementedException();
        }

        private bool CanMoveModTop(Mod obj)
        {
            throw new NotImplementedException();
        }

        private void MoveModUp(Mod obj)
        {
            throw new NotImplementedException();
        }

        private bool CanMoveModUp(Mod obj)
        {
            throw new NotImplementedException();
        }

        private void MoveModDown(Mod obj)
        {
            throw new NotImplementedException();
        }

        private bool CanMoveModDown(Mod obj)
        {
            throw new NotImplementedException();
        }

        private void MoveModBottom(Mod obj)
        {
            throw new NotImplementedException();
        }

        private bool CanMoveModBottom(Mod obj)
        {
            throw new NotImplementedException();
        }
    }
}
