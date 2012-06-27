#region usings

using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

#endregion

namespace Heath.Lister.Infrastructure.ViewModels
{
    public abstract class ViewModel : ViewModelBase
    {
        public abstract void Loaded();
    }
}