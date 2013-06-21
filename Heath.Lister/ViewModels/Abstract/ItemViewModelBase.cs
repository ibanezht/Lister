#region usings

using System;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Heath.Lister.Configuration;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.Models;
using Heath.Lister.Localization;
using Heath.Lister.Views;
using Telerik.Windows.Controls;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

#endregion

namespace Heath.Lister.ViewModels.Abstract
{
    public abstract class ItemViewModelBase : ViewModelBase
    {
        protected const string CompletedPropertyName = "Completed";
        protected const string CreatedDatePropertyName = "CreatedDate";
        protected const string DueDatePropertyName = "DueDate";
        protected const string DueDateTimePropertyName = "DueDateTime";
        protected const string DueTimePropertyName = "DueTime";
        protected const string IdPropertyName = "Id";
        protected const string ListColorPropertyName = "ListColor";
        protected const string ListIdPropertyName = "ListId";
        protected const string ListTitlePropertyName = "ListTitle";
        protected const string NotesPropertyName = "Notes";
        protected const string PriorityPropertyName = "Priority";
        protected const string ReminderDatePropertyName = "ReminderDate";
        protected const string ReminderTimePropertyName = "ReminderTime";
        protected const string TitlePropertyName = "Title";
        protected const string SelectedPropertyName = "Selected";

        private readonly INavigationService _navigationService;

        private ICommand _completeCommand;
        private bool _completed;
        private DateTime _createdDate;
        private ICommand _deleteCommand;
        private DateTime? _dueDate;
        private DateTime? _dueTime;
        private ICommand _editCommand;
        private Guid _id;
        private ICommand _incompleteCommand;
        private ColorViewModel _listColor;
        private Guid _listId;
        private string _listTitle;
        private string _notes;
        private ICommand _pinCommand;
        private Priority _priority;
        private bool _selected;
        private string _title;

        protected ItemViewModelBase(INavigationService navigationService)
        {
            if (IsInDesignMode)
            {
                Completed = false;
                DueDate = DateTime.Today.Date;
                DueTime = DateTime.Parse("10:00 PM");
                ListTitle = "design";
                Notes = "This is design mode data.";
                Title = "design mode";
            }

            _navigationService = navigationService;
        }

        public ICommand CompleteCommand
        {
            get { return _completeCommand ?? (_completeCommand = new RelayCommand(Complete, CanComplete)); }
        }

        public bool Completed
        {
            get { return _completed; }
            set
            {
                _completed = value;
                RaisePropertyChanged(CompletedPropertyName);
                ((RelayCommand)CompleteCommand).RaiseCanExecuteChanged();
                ((RelayCommand)IncompleteCommand).RaiseCanExecuteChanged();
            }
        }

        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set
            {
                _createdDate = value;
                RaisePropertyChanged(CreatedDatePropertyName);
            }
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = new RelayCommand(Delete)); }
        }

        public virtual DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                RaisePropertyChanged(DueDatePropertyName);
                RaisePropertyChanged(DueDateTimePropertyName);
            }
        }

        public string DueDateTime
        {
            get
            {
                var today = DateTime.Now.Date;
                var retval = AppResources.NotDueText;

                if (DueDate.HasValue && DueTime.HasValue)
                {
                    if (DueDate.Value.Date == today.Date)
                        retval = (DueDate.Value.Date + DueTime.Value.TimeOfDay).ToString("t");
                    else
                        retval = DueDate.Value.Date.ToString("M/dd");
                }
                else if (DueDate.HasValue)
                    retval = DueDate.Value.Date.ToString("M/dd");

                return retval;
            }
        }

        public virtual DateTime? DueTime
        {
            get { return _dueTime; }
            set
            {
                _dueTime = value;

                if (_dueTime != null && _dueDate == null)
                    _dueDate = DateTime.Today.Date;

                RaisePropertyChanged(DueTimePropertyName);
                RaisePropertyChanged(DueDatePropertyName);
                RaisePropertyChanged(DueDateTimePropertyName);
            }
        }

        public ICommand EditCommand
        {
            get { return _editCommand ?? (_editCommand = new RelayCommand(Edit)); }
        }

        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

        public ICommand IncompleteCommand
        {
            get { return _incompleteCommand ?? (_incompleteCommand = new RelayCommand(Incomplete, CanIncomplete)); }
        }

        public ColorViewModel ListColor
        {
            get { return _listColor; }
            set
            {
                _listColor = value;
                RaisePropertyChanged(ListColorPropertyName);
            }
        }

        public Guid ListId
        {
            get { return _listId; }
            set
            {
                _listId = value;
                RaisePropertyChanged(ListIdPropertyName);
            }
        }

        public string ListTitle
        {
            get { return _listTitle; }
            set
            {
                _listTitle = value;
                RaisePropertyChanged(ListTitlePropertyName);
            }
        }

        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                RaisePropertyChanged(NotesPropertyName);
            }
        }

        public ICommand PinCommand
        {
            get { return _pinCommand ?? (_pinCommand = new RelayCommand(Pin, CanPin)); }
        }

        public Priority Priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                RaisePropertyChanged(PriorityPropertyName);
            }
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                RaisePropertyChanged(SelectedPropertyName);
            }
        }

        public virtual string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(TitlePropertyName);
            }
        }

        public void Complete()
        {
            Completed = true;

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (sender, args) =>
            {
                using (var data = new DataAccess())
                    data.UpdateItem(Id, Completed);

                ScheduleReminderHelper.RemoveReminder(Id.ToString());

                // TODO: Shouldn't the pinned tiles also be removed??

                // TODO: Consider a dialog before delete/complete that warns about
                // removing reminders with a "don't show this again" check box.     
            };
            backgroundWorker.RunWorkerCompleted += CompleteCompleted;
            backgroundWorker.RunWorkerAsync();
        }

        private bool CanComplete()
        {
            return !Completed;
        }

        protected abstract void CompleteCompleted(object sender, RunWorkerCompletedEventArgs args);

        public void Delete()
        {
            Action delete = () =>
            {
                var backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += (sender, args) =>
                {
                    using (var data = new DataAccess())
                        data.DeleteItem(Id);

                    ScheduleReminderHelper.RemoveReminder(Id.ToString());

                    // TODO: need a base property for URI; 'new Uri' is duplicated 4 times in this class...
                    var shellTile = LiveTileHelper.GetTile(UriMappings.Instance.MapUri(new Uri(string.Format("/Item/{0}/{1}", Id, ListId), UriKind.Relative)));

                    if (shellTile != null)
                        shellTile.Delete();
                };
                backgroundWorker.RunWorkerCompleted += DeleteCompleted;
                backgroundWorker.RunWorkerAsync();
            };

            if (Selected)
                delete();

            else
            {
                Action<MessageBoxClosedEventArgs> closedHandler = e =>
                {
                    if (e.Result != DialogResult.OK)
                        return;

                    delete();
                };

                RadMessageBox.Show(AppResources.DeleteText, MessageBoxButtons.YesNo, AppResources.DeleteItemMessage, closedHandler: closedHandler);
            }
        }

        protected abstract void DeleteCompleted(object sender, RunWorkerCompletedEventArgs args);

        private void Edit()
        {
            _navigationService.Navigate(new Uri(string.Format("/EditItem/{0}/{1}", Id, ListId), UriKind.Relative));
            App.RemoveBackOnNext = true;
        }

        private void Incomplete()
        {
            Completed = false;

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (sender, args) =>
            {
                using (var data = new DataAccess())
                    data.UpdateItem(Id, Completed);

                ScheduleReminderHelper.RemoveReminder(Id.ToString());
            };
            backgroundWorker.RunWorkerCompleted += IncompleteCompleted;
            backgroundWorker.RunWorkerAsync();
        }

        private bool CanIncomplete()
        {
            return Completed;
        }

        protected abstract void IncompleteCompleted(object sender, RunWorkerCompletedEventArgs args);

        private void Pin()
        {
            var uri = UriMappings.Instance.MapUri(new Uri(string.Format("/Item/{0}/{1}", Id, ListId), UriKind.Relative));

            var itemFront = new ItemFrontView();
            itemFront.DataContext = this;
            itemFront.UpdateLayout();

            if (!string.IsNullOrEmpty(Notes))
            {
                var itemBack = new ItemBackView();
                itemBack.DataContext = this;
                itemBack.UpdateLayout();

                LiveTileHelper.CreateOrUpdateTile(new RadExtendedTileData { VisualElement = itemFront, BackVisualElement = itemBack }, uri);
            }

            else
                LiveTileHelper.CreateOrUpdateTile(new RadExtendedTileData { VisualElement = itemFront }, uri);
        }

        private bool CanPin()
        {
            return !Completed && LiveTileHelper.GetTile(UriMappings.Instance.MapUri(new Uri(string.Format("/Item/{0}/{1}", Id, ListId), UriKind.Relative))) == null;
        }

        protected void UpdatePin()
        {
            var uri = UriMappings.Instance.MapUri(new Uri(string.Format("/Item/{0}/{1}", Id, ListId), UriKind.Relative));

            var shellTile = LiveTileHelper.GetTile(uri);
            if (shellTile == null)
                return;

            var itemFront = new ItemFrontView();
            itemFront.DataContext = this;
            itemFront.UpdateLayout();

            if (!string.IsNullOrEmpty(Notes))
            {
                var itemBack = new ItemBackView();
                itemBack.DataContext = this;
                itemBack.UpdateLayout();

                LiveTileHelper.UpdateTile(shellTile, new RadExtendedTileData
                {
                    VisualElement = itemFront,
                    BackVisualElement = itemBack
                });
            }

            else
                LiveTileHelper.UpdateTile(shellTile, new RadExtendedTileData { VisualElement = itemFront });
        }
    }
}