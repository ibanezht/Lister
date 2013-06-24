#region usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.Extensions;
using Heath.Lister.Infrastructure.ViewModels;
using Heath.Lister.Localization;
using Heath.Lister.ViewModels.Abstract;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Telerik.Windows.Controls;
using MediaColor = System.Windows.Media.Color;

#endregion

namespace Heath.Lister.ViewModels
{
    public class ListViewModel : ListViewModelBase, IHaveId, IPageViewModel
    {
        private const string ListSortPropertyName = "ListSort";
        private const string SelectedPivotItemPropertyName = "SelectedPivotItem";
        private const string SortTitlePropertyName = "SortTitle";

        private readonly Func<ListItemViewModel> _createListItem;
        private readonly List<ListItemViewModel> _listItems = new List<ListItemViewModel>();

        private readonly Setting<ListSortViewModel> _listSortSetting
            = new Setting<ListSortViewModel>("ListSort", new ListSortViewModel());

        private readonly INavigationService _navigationService;

        private ICommand _addCommand;
        private ICommand _completeAllCommand;
        private ICommand _completeSelectedCommand;
        private ICommand _deleteSelectedCommand;
        private ICommand _selectCommand;
        private PivotItem _selectedPivotItem;
        private ICommand _shareCommand;
        private ICommand _sortCommand;
        private string _sortTitle;

        public ListViewModel(Func<ListItemViewModel> createListItem, INavigationService navigationService)
            : base(navigationService)
        {
            _createListItem = createListItem;
            _navigationService = navigationService;

            ApplicationTitle = "LISTER";

            Messenger.Default.Register<NotificationMessage<ListItemViewModel>>(this, ListItemNotificationMessageReceived);

            AllListItems = new ObservableCollection<ListItemViewModel>();
            TodayListItems = new ObservableCollection<ListItemViewModel>();
            OverdueListItems = new ObservableCollection<ListItemViewModel>();
        }

        public ICommand AddCommand
        {
            get { return _addCommand ?? (_addCommand = new RelayCommand(Add, CanAdd)); }
        }

        public ObservableCollection<ListItemViewModel> AllListItems { get; private set; }

        public string ApplicationTitle { get; private set; }

        public ICommand CompleteAllCommand
        {
            get { return _completeAllCommand ?? (_completeAllCommand = new RelayCommand(CompleteAll)); }
        }

        public ICommand CompleteSelectedCommand
        {
            get { return _completeSelectedCommand ?? (_completeSelectedCommand = new RelayCommand(CompleteSelected, CanCompleteSelected)); }
        }

        public ICommand DeleteSelectedCommand
        {
            get { return _deleteSelectedCommand ?? (_deleteSelectedCommand = new RelayCommand(DeleteSelected, CanDeleteSelected)); }
        }

        public ListSortViewModel ListSort { get; private set; }

        public ObservableCollection<ListItemViewModel> OverdueListItems { get; private set; }

        public ICommand SelectCommand
        {
            get { return _selectCommand ?? (_selectCommand = new RelayCommand(Select, CanSelect)); }
        }

        public PivotItem SelectedPivotItem
        {
            get { return _selectedPivotItem; }
            set
            {
                _selectedPivotItem = value;
                ((RelayCommand)SelectCommand).RaiseCanExecuteChanged();
                RaisePropertyChanged(SelectedPivotItemPropertyName);
            }
        }

        public ICommand ShareCommand
        {
            get { return _shareCommand ?? (_shareCommand = new RelayCommand(Share)); }
        }

        public bool ShowAdds
        {
            get { return App.AppMonetizationType == AppMonetizationType.Adds; }
        }

        public ICommand SortCommand
        {
            get { return _sortCommand ?? (_sortCommand = new RelayCommand<ApplicationBarButtonClickEventArgs>(Sort)); }
        }

        public string SortTitle
        {
            get { return _sortTitle; }
            set
            {
                _sortTitle = value;
                RaisePropertyChanged(SortTitlePropertyName);
            }
        }

        public ObservableCollection<ListItemViewModel> TodayListItems { get; private set; }

        #region IPageViewModel Members

        public void Activate()
        {
            if (App.RemoveBackEntry)
            {
                _navigationService.RemoveBackEntry();
                App.RemoveBackEntry = false;
            }

            _listItems.Clear();

            using (var data = new DataAccess())
            {
                var list = data.GetList(Id, true);

                Color = new ColorViewModel
                {
                    Id = list.Color.Id,
                    Text = list.Color.Text,
                    Color = MediaColor.FromArgb(255, list.Color.R, list.Color.G, list.Color.B)
                };

                CreatedDate = list.CreatedDate;
                Remaining = list.Items.Count(i => !i.Completed);
                Title = list.Title;

                list.Items.ForEach(i =>
                {
                    var listItem = _createListItem();
                    listItem.Completed = i.Completed;
                    listItem.CreatedDate = i.CreatedDate;
                    listItem.DueDate = i.DueDate;
                    listItem.DueTime = i.DueTime;
                    listItem.Id = i.Id;
                    listItem.ListColor = Color;
                    listItem.ListId = Id;
                    listItem.ListTitle = Title;
                    listItem.Notes = i.Notes;
                    listItem.Priority = i.Priority;
                    listItem.Title = i.Title;

                    _listItems.Add(listItem);
                });
            }

            LoadLists();
        }

        public void Deactivate(bool isNavigationInitiator)
        {
            UpdatePin();
        }

        public void ViewReady()
        {
            RateReminderHelper.Notify();
            TrialReminderHelper.Notify();

            ((RelayCommand)SelectCommand).RaiseCanExecuteChanged();
        }

        #endregion

        // TODO: Revisit these methods, everything else is a command and they're based off of CallMethodAction

        public void IsCheckModeActiveChanged()
        {
            _listItems.ForEach(li => li.Selected = false);
        }

        public void ItemCheckedStateChanged()
        {
            ((RelayCommand)CompleteSelectedCommand).RaiseCanExecuteChanged();
            ((RelayCommand)DeleteSelectedCommand).RaiseCanExecuteChanged();
        }

        public void ItemTap(ListBoxItemTapEventArgs e)
        {
            var listItemViewModel = (ListItemViewModel)e.Item.DataContext;

            _navigationService.Navigate(new Uri(string.Format("/Item/{0}/{1}", listItemViewModel.Id, listItemViewModel.ListId), UriKind.Relative));
        }

        public void PopupOpening()
        {
            ListSort = new ListSortViewModel
            {
                ListSortBy = _listSortSetting.Value.ListSortBy,
                ListSortDirection = _listSortSetting.Value.ListSortDirection,
                HideCompleted = _listSortSetting.Value.HideCompleted
            };
            RaisePropertyChanged(ListSortPropertyName);
        }

        // ENDTODO

        protected override void DeleteCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            if (_navigationService.CanGoBack())
                _navigationService.GoBack();

            else
            {
                _navigationService.Navigate(new Uri("/Hub", UriKind.Relative));
                App.RemoveBackEntry = true;
            }
        }

        private void ListItemNotificationMessageReceived(NotificationMessage<ListItemViewModel> notificationMessage)
        {
            switch (notificationMessage.Notification)
            {
                case "Complete":
                    var completedItem = notificationMessage.Content;

                    Remaining--;

                    if (_listSortSetting.Value.HideCompleted)
                    {
                        AllListItems.Remove(completedItem);
                        TodayListItems.Remove(completedItem);
                        OverdueListItems.Remove(completedItem);
                    }
                    break;

                case "Incomplete":
                    Remaining++;
                    break;

                case "Delete":
                    var deletedItem = notificationMessage.Content;

                    if (!deletedItem.Completed)
                        Remaining--;

                    _listItems.Remove(deletedItem);

                    AllListItems.Remove(deletedItem);
                    TodayListItems.Remove(deletedItem);
                    OverdueListItems.Remove(deletedItem);

                    ((RelayCommand)SelectCommand).RaiseCanExecuteChanged();
                    break;
            }
        }

        private void LoadLists()
        {
            IEnumerable<ListItemViewModel> sortedList = new List<ListItemViewModel>(_listItems);
            var sortTitleBuilder = new StringBuilder();

            sortTitleBuilder.AppendFormat("{0} ", AppResources.SortedByText);

            AllListItems.Clear();
            TodayListItems.Clear();
            OverdueListItems.Clear();

            if (_listSortSetting.Value.HideCompleted)
                sortedList = sortedList.Where(s => !s.Completed);

            if (_listSortSetting.Value.ListSortDirection == ListSortDirection.Ascending)
            {
                switch (_listSortSetting.Value.ListSortBy)
                {
                    case ListSortBy.Due:
                        sortedList = sortedList.OrderBy(l =>
                        {
                            if (l.DueDate.HasValue && l.DueTime.HasValue)
                                return l.DueDate.Value.Date + l.DueTime.Value.TimeOfDay;

                            return l.DueDate;
                        });
                        sortTitleBuilder.Append(AppResources.SortDueText);
                        break;

                    case ListSortBy.Title:
                        sortedList = sortedList.OrderBy(l => l.Title);
                        sortTitleBuilder.Append(AppResources.SortTitleText);
                        break;

                    case ListSortBy.Priority:
                        sortedList = sortedList.OrderBy(l => l.Priority);
                        sortTitleBuilder.Append(AppResources.SortPriorityText);
                        break;
                }

                sortTitleBuilder.AppendFormat(", {0}", AppResources.SortAscendingText);
            }
            else
            {
                switch (_listSortSetting.Value.ListSortBy)
                {
                    case ListSortBy.Due:
                        sortedList = sortedList.OrderByDescending(l =>
                        {
                            if (l.DueDate.HasValue && l.DueTime.HasValue)
                                return l.DueDate.Value.Date + l.DueTime.Value.TimeOfDay;

                            return l.DueDate;
                        });
                        sortTitleBuilder.Append(AppResources.SortDueText);
                        break;

                    case ListSortBy.Title:
                        sortedList = sortedList.OrderByDescending(l => l.Title);
                        sortTitleBuilder.Append(AppResources.SortTitleText);
                        break;

                    case ListSortBy.Priority:
                        sortedList = sortedList.OrderByDescending(l => l.Priority);
                        sortTitleBuilder.Append(AppResources.SortPriorityText);
                        break;
                }

                sortTitleBuilder.AppendFormat(", {0}", AppResources.SortDescendingText);
            }

            var today = DateTime.Now.Date;

            sortedList.ForEach(AllListItems.Add);
            sortedList.Where(li => li.DueDate.HasValue && li.DueDate.Value.Date == today).ForEach(TodayListItems.Add);
            sortedList.Where(li => li.DueDate.HasValue && li.DueDate.Value.Date < today).ForEach(OverdueListItems.Add);

            SortTitle = sortTitleBuilder.ToString();
        }

        private void Add()
        {
            _navigationService.Navigate(new Uri(string.Format("/EditItem/{0}/{1}", Guid.Empty, Id), UriKind.Relative));
        }

        private static bool CanAdd()
        {
            return !TrialReminderHelper.IsTrialExpired;
        }

        private void CompleteAll()
        {
            _listItems.ForEach(CompleteItem);
        }

        private void CompleteSelected()
        {
            _listItems.Where(l => l.Selected).ForEach(CompleteItem);

            ElementTreeHelper.FindVisualDescendant<RadDataBoundListBox>(SelectedPivotItem).IsCheckModeActive = false;
        }

        private bool CanCompleteSelected()
        {
            return _listItems.Any(l => l.Selected);
        }

        private static void CompleteItem(ListItemViewModel listItem)
        {
            if (listItem.Completed)
                return;

            listItem.Complete();
        }

        private void DeleteSelected()
        {
            var selectedItems = _listItems.Where(l => l.Selected).ToList();

            Action<MessageBoxClosedEventArgs> closedHandler = e =>
            {
                if (e.Result == DialogResult.OK)
                    selectedItems.ForEach(listItem => listItem.Delete());

                ElementTreeHelper.FindVisualDescendant<RadDataBoundListBox>(SelectedPivotItem).IsCheckModeActive = false;
            };

            string deleteItemsMessage;

            if (selectedItems.Count() > 1)
                deleteItemsMessage = AppResources.DeleteItemsMessage;
            else
                deleteItemsMessage = AppResources.DeleteItemMessage;

            RadMessageBox.Show(AppResources.DeleteText, MessageBoxButtons.YesNo, deleteItemsMessage, closedHandler: closedHandler);
        }

        private bool CanDeleteSelected()
        {
            return _listItems.Any(l => l.Selected);
        }

        private void Select()
        {
            ElementTreeHelper.FindVisualDescendant<RadDataBoundListBox>(SelectedPivotItem).IsCheckModeActive = true;
        }

        private bool CanSelect()
        {
            var radDataBoundListBox = ElementTreeHelper.FindVisualDescendant<RadDataBoundListBox>(SelectedPivotItem);

            if (radDataBoundListBox == null)
                return false;

            return radDataBoundListBox.ItemCount > 0;
        }

        private void Share()
        {
            Action<MessageBoxClosedEventArgs> closedHandler = e =>
            {
                switch (e.ButtonIndex)
                {
                    case 0:
                        var emailBuilder = new StringBuilder();

                        _listItems.ForEach(i =>
                        {
                            emailBuilder.AppendFormat("{0}, {1}", i.Title, i.DueDateTime);
                            emailBuilder.AppendLine();
                            
                            if (!string.IsNullOrEmpty(i.Notes))
                            {
                                emailBuilder.AppendFormat(" {0}", i.Notes);
                                emailBuilder.AppendLine();
                            }

                            emailBuilder.AppendLine();
                        });

                        var emailComposeTask = new EmailComposeTask();

                        emailComposeTask.Subject = Title;
                        emailComposeTask.Body = emailBuilder.ToString();
                        emailComposeTask.Show();

                        break;

                    case 1:
                        var textBuilder = new StringBuilder();

                        textBuilder.Append(Title);
                        textBuilder.AppendLine();

                        _listItems.ForEach(i =>
                        {
                            textBuilder.AppendFormat("{0}, {1}", i.Title, i.DueDateTime);
                            textBuilder.AppendLine();
                        });

                        var smsComposeTask = new SmsComposeTask();

                        smsComposeTask.Body = textBuilder.ToString();
                        smsComposeTask.Show();

                        break;
                }
            };

            RadMessageBox.Show(new[] { AppResources.EmailText, AppResources.TextText, AppResources.CancelText }, 
                AppResources.ShareText, AppResources.ShareListMessage, closedHandler: closedHandler);
        }

        private void Sort(ApplicationBarButtonClickEventArgs e)
        {
            if (e.Button.Text == AppResources.DoneText)
            {
                _listSortSetting.Value = ListSort;

                LoadLists();
            }

            Messenger.Default.Send(new NotificationMessage<ListViewModel>(this, "SortCompleted"));
        }
    }
}