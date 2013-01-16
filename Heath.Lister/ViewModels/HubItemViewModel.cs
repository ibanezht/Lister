#region usings

using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;
using Heath.Lister.Infrastructure;
using Heath.Lister.ViewModels.Abstract;

#endregion

namespace Heath.Lister.ViewModels
{
    public class HubItemViewModel : ListViewModelBase
    {
        public HubItemViewModel(INavigationService navigationService)
            : base(navigationService) { }

        protected override void DeleteCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            Messenger.Default.Send(new NotificationMessage<HubItemViewModel>(this, "Delete"));
        }
    }
}