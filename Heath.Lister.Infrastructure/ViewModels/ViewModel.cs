#region usings

using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

#endregion

namespace Heath.Lister.Infrastructure.ViewModels
{
    public abstract class ViewModel : ViewModelBase
    {
        private ICommand _loadedCommand;

        public ICommand LoadedCommand
        {
            get { return _loadedCommand ?? (_loadedCommand = new RelayCommand(Loaded)); }
        }

        protected abstract void Loaded();
    }
}