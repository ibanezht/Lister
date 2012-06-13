#region usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.ViewModels;
using Telerik.Windows.Controls;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

#endregion

namespace Heath.Lister.ViewModels
{
    public class ListPivotViewModel : ViewModelBase, IHaveHeader
    {
        private const string CanTapPropertyName = "CanTap";
        private const string IsCheckModeActivePropertyName = "IsCheckModeActive";
        private const string HeaderPropertyName = "Header";

        private readonly INavigationService _navigationService;

        private bool _canTap;
        private string _header;
        private bool _isCheckModeActive;

        public ListPivotViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            CheckModeChanged = new RelayCommand<IsCheckModeActiveChangedEventArgs>(IsCheckModeActiveChanged);
            CheckModeChanging = new RelayCommand<IsCheckModeActiveChangingEventArgs>(IsCheckModeActiveChanging);
            ItemTappedCommand = new RelayCommand<ListBoxItemTapEventArgs>(ListBoxItemTap, e => CanTap);

            CanTap = true;

            Messenger.Default.Register<NotificationMessage<IEnumerable<ListItemViewModel>>>(
                this, nm =>
                      {
                          if (nm.Notification == "Load")
                              Load(nm.Content);
                      });

            Messenger.Default.Register<NotificationMessage<ListItemViewModel>>(
                this, nm =>
                      {
                          if (nm.Notification == "Delete")
                              ListItems.Remove(ListItems.FirstOrDefault(li => li.Id == nm.Content.Id));
                      });

            ListItems = new ObservableCollection<ListItemViewModel>();
        }

        public bool CanTap
        {
            get { return _canTap; }
            set
            {
                _canTap = value;
                RaisePropertyChanged(CanTapPropertyName);
                ((RelayCommand<ListBoxItemTapEventArgs>)ItemTappedCommand).RaiseCanExecuteChanged();
            }
        }

        public bool IsCheckModeActive
        {
            get { return _isCheckModeActive; }
            set
            {
                _isCheckModeActive = value;
                RaisePropertyChanged(IsCheckModeActivePropertyName);
            }
        }

        public ObservableCollection<ListItemViewModel> ListItems { get; private set; }

        public Func<ListItemViewModel, bool> Filter { get; set; }

        public ICommand CheckModeChanged { get; private set; }

        public ICommand CheckModeChanging { get; private set; }

        public ICommand ItemTappedCommand { get; private set; }

        #region IHaveHeader Members

        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                RaisePropertyChanged(HeaderPropertyName);
            }
        }

        public string Tag { get; set; }

        #endregion

        private void IsCheckModeActiveChanged(IsCheckModeActiveChangedEventArgs e)
        {
            CanTap = !e.CheckBoxesVisible;
        }

        private void IsCheckModeActiveChanging(IsCheckModeActiveChangingEventArgs e)
        {
            CanTap = false;
        }

        private void ListBoxItemTap(ListBoxItemTapEventArgs e)
        {
            var listItemViewModel = (ListItemViewModel)e.Item.DataContext;

            _navigationService.Navigate(new Uri(string.Format("/Item/{0}/{1}", listItemViewModel.Id, listItemViewModel.ListId), UriKind.Relative));
        }

        private void Load(IEnumerable<ListItemViewModel> listItems)
        {
            ListItems.Clear();

            var listItemViewModels = listItems.Where(Filter).OrderBy(li => li.CreatedDate);

            if (listItemViewModels.Any())
                listItemViewModels.ToList().ForEach(ListItems.Add);
        }
    }
}