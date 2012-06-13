#region usings

using System.ComponentModel;
using System.Windows.Media;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.ViewModels;
using Heath.Lister.ViewModels.Abstract;

#endregion

namespace Heath.Lister.ViewModels
{
    public class ItemDetailsViewModel : ItemViewModelBase, IHaveListId, IViewModel
    {
        private readonly INavigationService _navigationService;

        public ItemDetailsViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;

            ApplicationTitle = "LISTER";
        }

        public string ApplicationTitle { get; private set; }

        #region IViewModel Members

        public void Activate()
        {
            using (var data = new ListerData())
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

        public void Deactivate(bool isNavigationInitiator) {}

        #endregion

        protected override void CompleteCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            if (_navigationService.CanGoBack())
                _navigationService.GoBack();
        }

        protected override void DeleteCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            if (_navigationService.CanGoBack())
                _navigationService.GoBack();
        }

        protected override void IncompleteCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            if (_navigationService.CanGoBack())
                _navigationService.GoBack();
        }
    }
}