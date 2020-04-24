﻿using Conflicted.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace Conflicted.ViewModel
{
    internal class ModFileViewModel : ModelViewModel<ModFile>
    {
        private static readonly Dictionary<ModFile, ModFileViewModel> instances = new Dictionary<ModFile, ModFileViewModel>();

        public string Path => model.Path;
        public string ID => model.ID;
        public string Name => model.Name;
        public string Extension => model.Extension;
        public string Directory => model.Directory;
        public string Text => model.Text;

        private ModViewModel mod;
        public ModViewModel Mod => mod ?? (mod = ModViewModel.Create(model.Mod));

        private IEnumerable<ModElementViewModel> elements;
        public IEnumerable<ModElementViewModel> Elements => elements ?? (elements = model.Elements.Select(element => ModElementViewModel.Create(element)).ToArray());

        private int? elementCount;
        public int? ElementCount => elementCount ?? (elementCount = Elements.Count());

        private IEnumerable<ModFileViewModel> conflicts;
        public IEnumerable<ModFileViewModel> Conflicts => conflicts ?? (conflicts = model.Conflicts.Select(file => Create(file)).ToArray());

        private int? conflictCount;
        public int? ConflictCount => conflictCount ?? (conflictCount = Conflicts.Count());

        private Brush conflictCountBrush;
        public Brush ConflictCountBrush => conflictCountBrush ?? (conflictCountBrush = ConflictCount > 0 ? Brushes.Red : Brushes.Black);

        private IEnumerable<ModFileViewModel> overwritten;
        public IEnumerable<ModFileViewModel> Overwritten => overwritten ?? (overwritten = model.Overwritten.Select(file => Create(file)).ToArray());

        private int? overwrittenCount;
        public int? OverwrittenCount => overwrittenCount ?? (overwrittenCount = Overwritten.Count());

        private IEnumerable<ModFileViewModel> overwriting;
        public IEnumerable<ModFileViewModel> Overwriting => overwriting ?? (overwriting = model.Overwriting.Select(file => Create(file)).ToArray());

        private int? overwritingCount;
        public int? OverwritingCount => overwritingCount ?? (overwritingCount = Overwriting.Count());

        private ModFileViewModel(ModFile model) : base(model)
        {
            instances[model] = this;
        }

        public static ModFileViewModel Create(ModFile model)
        {
            if (instances.ContainsKey(model))
            {
                return instances[model];
            }

            return new ModFileViewModel(model);
        }
    }
}