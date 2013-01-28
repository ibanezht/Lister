#region usings

using System;
using System.ComponentModel;
using System.Windows.Media;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.ViewModels;
using Heath.Lister.ViewModels.Abstract;

#endregion

namespace Heath.Lister.ViewModels
{
    public class ItemDetailsViewModel : ItemViewModelBase, IHaveListId, IPageViewModel
    {
        private readonly INavigationService _navigationService;

        public ItemDetailsViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;

            ApplicationTitle = "LISTER";
        }

        public string ApplicationTitle { get; private set; }

        public bool ShowAdds
        {
            get { return App.AppMonetizationType == AppMonetizationType.Adds; }
        }

        #region IPageViewModel Members

        public void Activate()
        {
            using (var data = new DataAccess())
            {
                var item = data.GetItem(Id);

                Completed = item.Completed;
                CreatedDate = item.CreatedDate;
                DueDate = item.DueDate;
                DueTime = item.DueTime;

                ListColor = new ColorViewModel
                {
                    Id = item.List.Color.Id,
                    Text = item.List.Color.Text,
                    Color = Color.FromArgb(255, item.List.Color.R, item.List.Color.G, item.List.Color.B)
                };

                ListTitle = item.List.Title;
                Notes = item.Notes;
                Priority = item.Priority;
                Title = item.Title;
            }
        }

        public void Deactivate(bool isNavigationInitiator) { }

        public void ViewReady()
        {
            RateReminderHelper.Notify();
            TrialReminderHelper.Notify();
        }

        #endregion

        protected override void CompleteCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            GoBack();
        }

        protected override void DeleteCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            GoBack();
        }

        protected override void IncompleteCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            GoBack();
        }

        private void GoBack()
        {
            if (_navigationService.CanGoBack())
                _navigationService.GoBack();

            else
            {
                _navigationService.Navigate(new Uri(string.Format("/List/{0}", ListId), UriKind.Relative));
                App.RemoveBackEntry = true;
            }
        }
    }
}