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
                selectedMod = value;
                OnPropertyChanged();
            }
        }

        private ModFileViewModel selectedFile;
        public ModFileViewModel SelectedFile
        {
            get => selectedFile;
            set
            {
                selectedFile = value;
                OnPropertyChanged();

                if (value == null)
                {
                    FileConflictsVisibility = Visibility.Collapsed;
                }
                else
                {
                    FileConflictsVisibility = Visibility.Visible;
                }
            }
        }

        private Visibility fileConflictsVisibility = Visibility.Collapsed;
        public Visibility FileConflictsVisibility
        {
            get => fileConflictsVisibility;
            set
            {
                fileConflictsVisibility = value;
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