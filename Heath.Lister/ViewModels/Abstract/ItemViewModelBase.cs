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
        protected const string DueTimePropertyName = "DueTime";
        protected const string IdPropertyName = "Id";
        protected const string ListColorPropertyName = "ListColor";
        protected const string ListIdPropertyName = "ListId";
        protected const string ListTitlePropertyName = "ListTitle";
        protected const string NotesPropertyName = "Notes";
        protected const string PriorityPropertyName = "Priority";
        protected const string TitlePropertyName = "Title";
        protected const string DueDateTimePropertyName = "DueDateTime";
        private const string SelectedPropertyName = "Selected";

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
        private ICommand _reminderCommand;
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
                ((RelayCommand)ReminderCommand).RaiseCanExecuteChanged();
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

        public DateTime? DueDate
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

        public DateTime? DueTime
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
                ((RelayCommand)ReminderCommand).RaiseCanExecuteChanged();
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

        public ICommand ReminderCommand
        {
            get { return _reminderCommand ?? (_reminderCommand = new RelayCommand(Remind, CanRemind)); }
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

        public string Title
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
            backgroundWorker.DoWork +=
                (sender, args) =>
                {
                    using (var data = new DataAccess())
                        data.UpdateItem(Id, Completed);

                    ScheduleReminderHelper.RemoveReminder(Id.ToString());
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
            Action delete =
                () =>
                {
                    var backgroundWorker = new BackgroundWorker();
                    backgroundWorker.DoWork +=
                        (sender, args) =>
                        {
                            using (var data = new DataAccess())
                                data.DeleteItem(Id);

                            ScheduleReminderHelper.RemoveReminder(Id.ToString());
                        };
                    backgroundWorker.RunWorkerCompleted += DeleteCompleted;
                    backgroundWorker.RunWorkerAsync();
                };

            if (Selected)
                delete();

            else
            {
                Action<MessageBoxClosedEventArgs> closedHandler =
                    e =>
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
        }

        private void Incomplete()
        {
            Completed = false;

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork +=
                (sender, args) =>
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

        private void Remind()
        {
            var attemptedDate = DateTime.Now;

            if (DueDate.HasValue && DueTime.HasValue)
                attemptedDate = DueDate.Value.Date + DueTime.Value.TimeOfDay;
            else if (DueDate.HasValue)
                attemptedDate = DueDate.Value.Date;

            var fallbackDate = DateTime.Now.AddMinutes(5);

            var reminderDate = fallbackDate > attemptedDate ? fallbackDate : attemptedDate;
            var uri = UriMappings.Instance.MapUri(new Uri(string.Format("/Item/{0}/{1}", Id, ListId), UriKind.Relative));

            Action<MessageBoxClosedEventArgs> closedHandler =
                e =>
                {
                    if (e.Result == DialogResult.OK)
                    {
                        ScheduleReminderHelper.AddReminder(Id.ToString(), uri, Title, Notes ?? string.Empty, reminderDate);

                        ((RelayCommand)ReminderCommand).RaiseCanExecuteChanged();
                    }
                };

            RadMessageBox.Show(AppResources.ReminderText, MessageBoxButtons.OK, string.Format(AppResources.ReminderMessageText, reminderDate), closedHandler: closedHandler);
        }

        private bool CanRemind()
        {
            return !Completed && !ScheduleReminderHelper.HasReminder(Id.ToString());
        }

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
    }
}