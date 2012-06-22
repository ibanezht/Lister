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
    public class ListViewModel : ListViewModelBase, IHaveId, IViewModel
    {
        private const string SelectedPivotItemPropertyName = "SelectedPivotItem";

        private readonly Func<ListItemViewModel> _createListItem;
        private readonly List<ListItemViewModel> _listItems = new List<ListItemViewModel>();
        private readonly INavigationService _navigationService;

        private ICommand _addCommand;
        private ICommand _completeAllCommand;
        private ICommand _completeSelectedCommand;
        private ICommand _deleteSelectedCommand;
        private ICommand _selectCommand;
        private PivotItem _selectedPivotItem;
        private ICommand _shareCommand;

        public ListViewModel(Func<ListItemViewModel> createListItem, INavigationService navigationService)
            : base(navigationService)
        {
            _createListItem = createListItem;
            _navigationService = navigationService;

            ApplicationTitle = "LISTER";

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

        public ObservableCollection<ListItemViewModel> TodayListItems { get; private set; }

        #region IViewModel Members

        public void Activate()
        {
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

            var today = DateTime.Now.Date;

            AllListItems.Clear();
            TodayListItems.Clear();
            OverdueListItems.Clear();

            _listItems.ForEach(AllListItems.Add);
            _listItems.Where(li => li.DueDate.HasValue && li.DueDate.Value.Date == today).ToList().ForEach(TodayListItems.Add);
            _listItems.Where(li => li.DueDate.HasValue && li.DueDate.Value.Date < today).ToList().ForEach(OverdueListItems.Add);
        }

        public void Deactivate(bool isNavigationInitiator)
        {
            UpdatePin(isNavigationInitiator);
        }

        #endregion

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

        //private void ListItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Remove)
        //        ((RelayCommand)SelectCommand).RaiseCanExecuteChanged();
        //}

        //private void PivotItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "IsCheckModeActive")
        //    {
        //        _listItems.ForEach(li => li.Selected = false);

        //        OnIsCheckModeActiveChanged(new IsCheckModeActiveChangedEventArgs(((ListPivotViewModel)sender).IsCheckModeActive));
        //    }
        //}

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
            _listItems.Where(l => l.Selected).ToList().ForEach(CompleteItem);

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

            Action<MessageBoxClosedEventArgs> closedHandler =
                e =>
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
    }
}