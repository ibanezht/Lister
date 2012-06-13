#region usings

using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;
using Heath.Lister.Infrastructure;
using Heath.Lister.ViewModels.Abstract;

#endregion

namespace Heath.Lister.ViewModels
{
    public class ListItemViewModel : ItemViewModelBase
    {
        public ListItemViewModel(INavigationService navigationService)
            : base(navigationService) {}

        protected override void CompleteCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            Messenger.Default.Send(new NotificationMessage<ListItemViewModel>(this, "Complete"));
        }

        protected override void DeleteCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            Messenger.Default.Send(new NotificationMessage<ListItemViewModel>(this, "Delete"));
        }

        protected override void IncompleteCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            Messenger.Default.Send(new NotificationMessage<ListItemViewModel>(this, "Incomplete"));
        }
    }
}