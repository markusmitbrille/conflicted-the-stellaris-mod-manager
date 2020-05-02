using System;
using System.Collections.Generic;

namespace Conflicted.ViewModels
{
    abstract class ModelViewModel<TModel> : BaseViewModel where TModel : class
    {
        protected TModel Model { get; private set; }

        public ModelViewModel(TModel model) : base()
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }
    }
}