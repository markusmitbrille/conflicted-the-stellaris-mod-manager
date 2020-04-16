using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Conflicted.ViewModel
{
    internal class BaseViewModel : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Methods

        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion Methods
    }
}