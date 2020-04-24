﻿using Conflicted.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace Conflicted.ViewModel
{
    internal class ModElementViewModel : ModelViewModel<ModElement>
    {
        private static readonly Dictionary<ModElement, ModElementViewModel> instances = new Dictionary<ModElement, ModElementViewModel>();

        public string ID => model.ID;
        public string Text => model.Text;

        private ModFileViewModel file;
        public ModFileViewModel File => file ?? (file = ModFileViewModel.Create(model.File));

        private ModViewModel mod;
        public ModViewModel Mod => mod ?? (mod = ModViewModel.Create(model.Mod));

        private IEnumerable<ModElementViewModel> conflicts;
        public IEnumerable<ModElementViewModel> Conflicts => conflicts ?? (conflicts = model.Conflicts.Select(file => Create(file)).ToArray());

        private int? conflictCount;
        public int? ConflictCount => conflictCount ?? (conflictCount = Conflicts.Count());

        private Brush conflictCountBrush;
        public Brush ConflictCountBrush => conflictCountBrush ?? (conflictCountBrush = ConflictCount > 0 ? Brushes.Red : Brushes.Black);

        private ModElementViewModel(ModElement model) : base(model)
        {
            instances[model] = this;
        }

        public static ModElementViewModel Create(ModElement model)
        {
            if (instances.ContainsKey(model))
            {
                return instances[model];
            }

            return new ModElementViewModel(model);
        }
    }
}