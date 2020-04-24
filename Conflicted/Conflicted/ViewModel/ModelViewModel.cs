using System;
using System.Collections.Generic;

namespace Conflicted.ViewModel
{
    internal abstract class ModelViewModel<TModel> : BaseViewModel where TModel : class
    {
        protected readonly TModel model;

        public ModelViewModel(TModel model) : base()
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
        }
    }
}