using Conflicted.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Conflicted.ViewModel
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private ModlistViewModel modlist;
        public ModlistViewModel Modlist => modlist ?? (modlist = ModlistViewModel.Create(new Modlist()));

        private ModViewModel selectedMod;
        public ModViewModel SelectedMod
        {
            get => selectedMod;
            set
            {
                SelectedFile = null;
                SelectedFileConflict = null;

                SelectedElement = null;
                SelectedElementConflict = null;

                selectedMod = value;

                OnPropertyChanged();

                OnPropertyChanged(nameof(MoveButtonIsEnabled));

                OnPropertyChanged(nameof(FileTabHeader));
                OnPropertyChanged(nameof(ElementTabHeader));

                OnPropertyChanged(nameof(FileTabVisibility));
                OnPropertyChanged(nameof(ElementTabVisibility));
            }
        }

        public bool MoveButtonIsEnabled => SelectedMod != null;

        public string FileTabHeader => SelectedMod == null ? null : $"{SelectedMod.FileCount} {(SelectedMod.FileCount > 1 ? "Files" : "File")}{(SelectedMod.FileConflictCount > 0 ? $" with {SelectedMod.FileConflictCount} {(SelectedMod.FileConflictCount > 1 ? "Conflicts" : "Conflict")}" : null)}";
        public string ElementTabHeader => SelectedMod == null ? null : $"{SelectedMod.ElementCount} {(SelectedMod.ElementCount > 1 ? "Elements" : "Element")}{(SelectedMod.ElementConflictCount > 0 ? $" with {SelectedMod.ElementConflictCount} {(SelectedMod.ElementConflictCount > 1 ? "Conflicts" : "Conflict")}" : null)}";

        public Visibility FileTabVisibility => SelectedMod == null ? Visibility.Collapsed : SelectedMod.FileCount > 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ElementTabVisibility => SelectedMod == null ? Visibility.Collapsed : SelectedMod.ElementCount > 0 ? Visibility.Visible : Visibility.Collapsed;

        private ModFileViewModel selectedFile;
        public ModFileViewModel SelectedFile
        {
            get => selectedFile;
            set
            {
                selectedFile = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(FileConflictColumnWidth));
                OnPropertyChanged(nameof(FileContentRowHeight));
            }
        }

        public GridLength FileConflictColumnWidth => SelectedFile == null ? new GridLength(0, GridUnitType.Star) : SelectedFile.ConflictCount > 0 ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Star);
        public GridLength FileContentRowHeight => SelectedFile == null ? new GridLength(0, GridUnitType.Star) : string.IsNullOrEmpty(SelectedFile.Text) ? new GridLength(0, GridUnitType.Star) : new GridLength(1, GridUnitType.Star);

        private ModFileViewModel selectedFileConflict;
        public ModFileViewModel SelectedFileConflict
        {
            get => selectedFileConflict;
            set
            {
                selectedFileConflict = value;
                OnPropertyChanged();
            }
        }

        private ModElementViewModel selectedElement;
        public ModElementViewModel SelectedElement
        {
            get => selectedElement;
            set
            {
                selectedElement = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ElementConflictColumnWidth));
                OnPropertyChanged(nameof(ElementContentRowHeight));
            }
        }

        public GridLength ElementConflictColumnWidth => SelectedElement == null ? new GridLength(0, GridUnitType.Star) : SelectedElement.ConflictCount > 0 ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Star);
        public GridLength ElementContentRowHeight => SelectedElement == null ? new GridLength(0, GridUnitType.Star) : string.IsNullOrEmpty(SelectedElement.Text) ? new GridLength(0, GridUnitType.Star) : new GridLength(1, GridUnitType.Star);

        private ModElementViewModel selectedElementConflict;
        public ModElementViewModel SelectedElementConflict
        {
            get => selectedElementConflict;
            set
            {
                selectedElementConflict = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand openOptionsCommand;
        public RelayCommand OpenOptionsCommand => openOptionsCommand ?? (openOptionsCommand = new RelayCommand(ExecuteOpenOptions));

        public MainWindowViewModel()
        {
        }

        private void ExecuteOpenOptions(object obj)
        {
            throw new NotImplementedException();
        }
    }
}