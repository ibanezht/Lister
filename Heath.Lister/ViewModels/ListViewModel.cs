#region usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.ViewModels;
using Heath.Lister.Localization;
using Heath.Lister.ViewModels.Abstract;
using Microsoft.Phone.Tasks;
using Telerik.Windows.Controls;
using MediaColor = System.Windows.Media.Color;

#endregion

namespace Heath.Lister.ViewModels
{
    public class ListViewModel : ListViewModelBase, IHaveId, IViewModel
    {
        private const string SelectedListPivotItemPropertyName = "SelectedListPivotItem";

        private readonly Func<ListItemViewModel> _createListItem;
        private readonly Func<ListPivotViewModel> _createListPivot;
        private readonly List<ListItemViewModel> _listItems = new List<ListItemViewModel>();
        private readonly INavigationService _navigationService;

        private ListPivotViewModel _selectedListPivotItem;

        public ListViewModel(Func<ListPivotViewModel> createListPivot, Func<ListItemViewModel> createListItem, INavigationService navigationService)
            : base(navigationService)
        {
            _createListPivot = createListPivot;
            _createListItem = createListItem;
            _navigationService = navigationService;

            ApplicationTitle = "LISTER";

            AddCommand = new RelayCommand(Add, () => !TrialReminderHelper.IsTrialExpired);
            DeleteSelectedCommand = new RelayCommand(DeleteSelected, () => _listItems.Any(l => l.Selected));
            CompleteAllCommand = new RelayCommand(CompleteAll);
            CompleteSelectedCommand = new RelayCommand(CompleteSelected, () => _listItems.Any(l => l.Selected));
            SelectCommand = new RelayCommand(Select, () => _selectedListPivotItem != null && _selectedListPivotItem.ListItems.Any());
            ShareCommand = new RelayCommand(Share);

            Messenger.Default.Register<NotificationMessage<ListItemViewModel>>(
                this, nm =>
                      {
                          if (nm.Notification == "Complete")
                              Remaining--;

                          if (nm.Notification == "Delete" && !nm.Content.Completed)
                              Remaining--;

                          if (nm.Notification == "Incomplete")
                              Remaining++;
                      });

            ListPivotItems = new ObservableCollection<ListPivotViewModel>();
        }

        public ICommand AddCommand { get; private set; }

        public string ApplicationTitle { get; private set; }

        public ICommand CompleteAllCommand { get; private set; }

        public ICommand CompleteSelectedCommand { get; private set; }

        public ICommand DeleteSelectedCommand { get; private set; }

        public ObservableCollection<ListPivotViewModel> ListPivotItems { get; private set; }

        public ICommand SelectCommand { get; private set; }

        public ListPivotViewModel SelectedListPivotItem
        {
            get { return _selectedListPivotItem; }
            set
            {
                _selectedListPivotItem = value;

                RaisePropertyChanged(SelectedListPivotItemPropertyName);
                ((RelayCommand)SelectCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand ShareCommand { get; private set; }

        #region IViewModel Members

        public void Activate()
        {
            ListPivotItems.Clear();

            var today = DateTime.Now.Date;

            var allListPivot = _createListPivot();
            allListPivot.Header = AppResources.AllText;
            allListPivot.Filter = _ => { return true; };
            allListPivot.ListItems.CollectionChanged += ListItemsCollectionChanged;
            allListPivot.PropertyChanged += PivotItemPropertyChanged;

            ListPivotItems.Add(allListPivot);

            var todayListPivot = _createListPivot();
            todayListPivot.Header = AppResources.TodayText;
            todayListPivot.Filter = li => { return li.DueDate.HasValue && li.DueDate.Value.Date == today; };
            todayListPivot.PropertyChanged += PivotItemPropertyChanged;

            ListPivotItems.Add(todayListPivot);

            var overdueListPivot = _createListPivot();
            overdueListPivot.Header = AppResources.OverdueText;
            overdueListPivot.Filter = li => { return li.DueDate.HasValue && li.DueDate.Value.Date < today; };
            overdueListPivot.PropertyChanged += PivotItemPropertyChanged;

            ListPivotItems.Add(overdueListPivot);

            _listItems.Clear();

            using (var data = new ListerData())
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

                list.Items.ToList().ForEach(
                    i =>
                    {
                        var listItem = _createListItem();
                        listItem.Completed = i.Completed;
                        listItem.CreatedDate = i.CreatedDate;
                        listItem.DueDate = i.DueDate;
                        listItem.DueTime = i.DueTime;
                        listItem.Id = i.Id;
                        listItem.ListColor = Color;
                        listItem.ListId = Id;
                        listItem.Notes = i.Notes;
                        listItem.Priority = i.Priority;
                        listItem.Title = i.Title;

                        listItem.PropertyChanged += ListItemPropertyChanged;

                        _listItems.Add(listItem);
                    });
            }

            Messenger.Default.Send(new NotificationMessage<IEnumerable<ListItemViewModel>>(_listItems, "Load"));

            SelectedListPivotItem = allListPivot;
        }

        public void Deactivate(bool isNavigationInitiator)
        {
            UpdatePin(isNavigationInitiator);
        }

        #endregion

        public event EventHandler<IsCheckModeActiveChangedEventArgs> IsCheckModeActiveChanged;

        protected override void DeleteCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            _navigationService.GoBack();
        }

        private void ListItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Selected") 
                return;

            ((RelayCommand)CompleteSelectedCommand).RaiseCanExecuteChanged();
            ((RelayCommand)DeleteSelectedCommand).RaiseCanExecuteChanged();
        }

        private void ListItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
                ((RelayCommand)SelectCommand).RaiseCanExecuteChanged();
        }

        private void PivotItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsCheckModeActive")
            {
                _listItems.ForEach(li => li.Selected = false);

                OnIsCheckModeActiveChanged(new IsCheckModeActiveChangedEventArgs(((ListPivotViewModel)sender).IsCheckModeActive));
            }
        }

        private void Add()
        {
            _navigationService.Navigate(new Uri(string.Format("/EditItem/{0}/{1}", Guid.Empty, Id), UriKind.Relative));
        }

        private void CompleteAll()
        {
            _listItems.ForEach(CompleteItem);
        }

        private void CompleteSelected()
        {
            _listItems.Where(l => l.Selected).ToList().ForEach(CompleteItem);

            _selectedListPivotItem.IsCheckModeActive = false;
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

            Action<MessageBoxClosedEventArgs> closedHandler =
                e =>
                {
                    if (e.Result == DialogResult.OK)
                        selectedItems.ForEach(listItem => listItem.Delete());

                    _selectedListPivotItem.IsCheckModeActive = false;
                };

            string deleteItemsMessage;

            if (selectedItems.Count() > 1)
                deleteItemsMessage = AppResources.DeleteItemsMessage;
            else
                deleteItemsMessage = AppResources.DeleteItemMessage;

            RadMessageBox.Show(AppResources.DeleteText, MessageBoxButtons.YesNo, deleteItemsMessage, closedHandler: closedHandler);
        }

        private void Select()
        {
            _selectedListPivotItem.IsCheckModeActive = true;
        }

        private void Share()
        {
            var builder = new StringBuilder();
            builder.Append(Title);
            builder.AppendLine();

            _listItems.ToList().ForEach(
                i =>
                {
                    builder.AppendFormat(" {0}", i.Title);
                    builder.AppendLine();
                });

            var task = new SmsComposeTask();
            task.Body = builder.ToString();
            task.Show();
        }

        protected void OnIsCheckModeActiveChanged(IsCheckModeActiveChangedEventArgs e)
        {
            if (IsCheckModeActiveChanged != null)
            {
                IsCheckModeActiveChanged(this, e);
            }
        }
    }
}