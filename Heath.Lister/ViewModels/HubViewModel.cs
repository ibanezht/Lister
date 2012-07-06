#region usings

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.Extensions;
using Heath.Lister.Infrastructure.ViewModels;
using Telerik.Windows.Controls;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

#endregion

namespace Heath.Lister.ViewModels
{
    public class HubViewModel : ViewModelBase, IPageViewModel
    {
        private readonly Func<HubItemViewModel> _createHubItem;
        private readonly INavigationService _navigationService;

        private ICommand _aboutCommand;
        private ICommand _addCommand;
        private ICommand _itemTappedCommand;

        public HubViewModel(Func<HubItemViewModel> createHubItem, INavigationService navigationService)
        {
            _createHubItem = createHubItem;
            _navigationService = navigationService;

            ApplicationTitle = "LISTER";

            Messenger.Default.Register<NotificationMessage<HubItemViewModel>>(
                this, nm =>
                      {
                          if (nm.Notification == "Delete")
                              HubItems.Remove(HubItems.First(hi => hi.Id == nm.Content.Id));
                      });

            HubItems = new ObservableCollection<HubItemViewModel>();
        }

        public string ApplicationTitle { get; private set; }

        public ObservableCollection<HubItemViewModel> HubItems { get; private set; }

        public ICommand AboutCommand
        {
            get { return _aboutCommand ?? (_aboutCommand = new RelayCommand(About)); }
        }

        public ICommand AddCommand
        {
            get { return _addCommand ?? (_addCommand = new RelayCommand(Add, CanAdd)); }
        }

        public ICommand ItemTappedCommand
        {
            get { return _itemTappedCommand ?? (_itemTappedCommand = new RelayCommand<ListBoxItemTapEventArgs>(ItemTapped)); }
        }

        #region IPageViewModel Members

        public void Activate()
        {
            HubItems.Clear();

            using (var data = new DataAccess())
            {
                data.GetLists().OrderBy(l => l.CreatedDate).ForEach(
                    l =>
                    {
                        var hubItem = _createHubItem();
                        hubItem.Color = new ColorViewModel
                                        {
                                            Id = l.Color.Id,
                                            Text = l.Color.Text,
                                            Color = Color.FromArgb(255, l.Color.R, l.Color.G, l.Color.B)
                                        };
                        hubItem.CreatedDate = l.CreatedDate;
                        hubItem.Id = l.Id;
                        hubItem.Remaining = l.Items.Count(i => !i.Completed);
                        hubItem.Title = l.Title;
                        HubItems.Add(hubItem);
                    });
            }
        }

        public void Deactivate(bool isNavigationInitiator) {}

        public void ViewReady()
        {
            RateReminderHelper.Notify();
            TrialReminderHelper.Notify();
        }

        #endregion

        private void About()
        {
            _navigationService.Navigate(new Uri("/About", UriKind.Relative));
        }

        private void Add()
        {
            _navigationService.Navigate(new Uri(string.Format("/EditList/{0}", Guid.Empty), UriKind.Relative));
        }

        private static bool CanAdd()
        {
            return !TrialReminderHelper.IsTrialExpired;
        }

        private void ItemTapped(ListBoxItemTapEventArgs e)
        {
            var hubItemViewModel = (HubItemViewModel)e.Item.DataContext;

            _navigationService.Navigate(new Uri(string.Format("/List/{0}", hubItemViewModel.Id), UriKind.Relative));
        }
    }
}