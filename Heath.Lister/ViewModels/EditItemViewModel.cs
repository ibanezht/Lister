﻿#region usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Heath.Lister.Configuration;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.Models;
using Heath.Lister.Infrastructure.ViewModels;
using Heath.Lister.Localization;
using Heath.Lister.ViewModels.Abstract;
using Telerik.Windows.Controls;
using Color = System.Windows.Media.Color;

#endregion

namespace Heath.Lister.ViewModels
{
    public class EditItemViewModel : ItemViewModelBase, IHaveListId, IPageViewModel
    {
        private const string PageNamePropertyName = "PageName";
        private const string ReminderPropertyName = "Reminder";

        private readonly INavigationService _navigationService;

        private ICommand _cancelCommand;
        private ICommand _clearDateCommand;
        private ICommand _clearTimeCommand;
        private DateTime? _dueDate;
        private DateTime? _dueTime;
        private string _pageName;
        private bool _reminder;
        private DateTime? _reminderDate;
        private DateTime? _reminderTime;
        private ICommand _saveCommand;
        private ICommand _saveNextCommand;

        public EditItemViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;

            ApplicationTitle = "LISTER";
        }

        public string ApplicationTitle { get; private set; }

        public override DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;

                if (ReminderDate == null)
                    ReminderDate = _dueDate;

                RaisePropertyChanged(DueDatePropertyName);
                RaisePropertyChanged(DueDateTimePropertyName);
            }
        }

        public override DateTime? DueTime
        {
            get { return _dueTime; }
            set
            {
                _dueTime = value;

                if (_dueTime != null && _dueDate == null)
                    _dueDate = DateTime.Today.Date;

                if (ReminderTime == null)
                    ReminderTime = _dueTime;

                RaisePropertyChanged(DueTimePropertyName);
                RaisePropertyChanged(DueDatePropertyName);
                RaisePropertyChanged(DueDateTimePropertyName);
            }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(Cancel)); }
        }

        public bool CanRemind
        {
            get { return !Completed && !ScheduleReminderHelper.HasReminder(Id.ToString()); }
        }

        public ICommand ClearDateCommand
        {
            get { return _clearDateCommand ?? (_clearDateCommand = new RelayCommand(ClearDate)); }
        }

        public ICommand ClearTimeCommand
        {
            get { return _clearTimeCommand ?? (_clearTimeCommand = new RelayCommand(ClearTime)); }
        }

        public string PageName
        {
            get { return _pageName; }
            set
            {
                _pageName = value;
                RaisePropertyChanged(PageNamePropertyName);
            }
        }

        public bool Reminder
        {
            get { return _reminder; }
            set
            {
                _reminder = value;
                RaisePropertyChanged(ReminderPropertyName);
            }
        }

        public DateTime? ReminderDate
        {
            get { return _reminderDate; }
            set
            {
                _reminderDate = value;
                RaisePropertyChanged(ReminderDatePropertyName);
            }
        }

        public DateTime? ReminderTime
        {
            get { return _reminderTime; }
            set
            {
                _reminderTime = value;
                RaisePropertyChanged(ReminderTimePropertyName);
            }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save, CanSave)); }
        }

        public ICommand SaveNextCommand
        {
            get { return _saveNextCommand ?? (_saveNextCommand = new RelayCommand(SaveNext, CanSave)); }
        }

        public IEnumerable<string> Suggestions
        {
            get
            {
                IEnumerable<string> retval;

                using (var data = new DataAccess())
                    retval = data.GetItemTitles();

                return retval;
            }
        }

        public override string Title
        {
            get { return base.Title; }
            set
            {
                base.Title = value;
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                ((RelayCommand)SaveNextCommand).RaiseCanExecuteChanged();
            }
        }

        #region IPageViewModel Members

        public void Activate()
        {
            var completed = TombstoningHelper.Load<bool>(CompletedPropertyName);
            var dueDate = TombstoningHelper.Load<DateTime?>(DueDatePropertyName);
            var dueTime = TombstoningHelper.Load<DateTime?>(DueTimePropertyName);
            var notes = TombstoningHelper.Load<string>(NotesPropertyName);
            var priority = TombstoningHelper.Load(PriorityPropertyName, Priority.None);
            var title = TombstoningHelper.Load<string>(TitlePropertyName);

            if (Id != Guid.Empty)
            {
                PageName = string.Format("{0} {1}", AppResources.EditText, AppResources.ItemText);

                using (var data = new DataAccess())
                {
                    var item = data.GetItem(Id);

                    completed = item.Completed;
                    CreatedDate = item.CreatedDate;
                    dueDate = item.DueDate;
                    dueTime = item.DueTime;

                    ListColor = new ColorViewModel
                    {
                        Id = item.List.Color.Id,
                        Text = item.List.Color.Text,
                        Color = Color.FromArgb(255, item.List.Color.R, item.List.Color.G, item.List.Color.B)
                    };

                    ListTitle = item.List.Title;
                    notes = item.Notes;
                    priority = item.Priority;
                    title = item.Title;
                }
            }
            else
            {
                PageName = string.Format("{0} {1}", AppResources.AddText, AppResources.ItemText);

                using (var data = new DataAccess())
                {
                    var list = data.GetList(ListId, false);

                    ListColor = new ColorViewModel
                    {
                        Id = list.Color.Id,
                        Text = list.Color.Text,
                        Color = Color.FromArgb(255, list.Color.R, list.Color.G, list.Color.B)
                    };

                    ListTitle = list.Title;
                }
            }

            Completed = completed;
            DueDate = dueDate;
            DueTime = dueTime;
            Notes = notes;
            Priority = priority;
            Title = title;
        }

        public void Deactivate(bool isNavigationInitiator)
        {
            if (isNavigationInitiator)
                TombstoningHelper.Clear();

            else
            {
                TombstoningHelper.Save(CompletedPropertyName, Completed);
                TombstoningHelper.Save(DueDatePropertyName, DueDate);
                TombstoningHelper.Save(DueTimePropertyName, DueTime);
                TombstoningHelper.Save(NotesPropertyName, Notes);
                TombstoningHelper.Save(PriorityPropertyName, Priority);
                TombstoningHelper.Save(TitlePropertyName, Title);
            }
        }

        public void ViewReady() { }

        #endregion

        protected override void CompleteCompleted(object sender, RunWorkerCompletedEventArgs args) { }

        protected override void DeleteCompleted(object sender, RunWorkerCompletedEventArgs args) { }

        protected override void IncompleteCompleted(object sender, RunWorkerCompletedEventArgs args) { }

        private void Cancel()
        {
            GoBack();
        }

        private void ClearDate()
        {
            DueDate = null;
            DueTime = null;
        }

        private void ClearTime()
        {
            DueTime = null;
        }

        private void Save()
        {
            SaveInternal(true);
        }

        private void SaveNext()
        {
            SaveInternal(false);
        }

        private void SaveInternal(bool goBack)
        {
            Action<DateTime?> save = r =>
            {
                var backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += (sender, args) =>
                {
                    Item item;

                    using (var data = new DataAccess())
                        item = data.UpsertItem(Id, ListId, Completed, DueDate, DueTime, Notes, Priority, Title);

                    if (r.HasValue)
                    {
                        var uri = UriMappings.Instance.MapUri(new Uri(string.Format("/Item/{0}/{1}", item.Id, ListId), UriKind.Relative));

                        ScheduleReminderHelper.AddReminder(item.Id.ToString(), uri, Title, Notes ?? string.Empty, r.Value);
                    }

                    DispatcherHelper.UIDispatcher.BeginInvoke(UpdatePin);
                };

                backgroundWorker.RunWorkerCompleted += (sender, args) =>
                {
                    if (goBack)
                        GoBack();

                    else
                    {
                        if (App.RemoveBackOnNext && Id != Guid.Empty)
                            _navigationService.RemoveBackEntry();

                        Id = Guid.Empty;
                        Completed = false;
                        DueDate = null;
                        DueTime = null;
                        Notes = null;
                        Priority = Priority.None;
                        Title = null;
                        ReminderDate = null;
                        ReminderTime = null;
                    }
                };
                backgroundWorker.RunWorkerAsync();
            };

            if (Reminder)
            {
                Action<MessageBoxClosedEventArgs> closedHandler = e =>
                {
                    if (e.Result != DialogResult.OK)
                        return;

                    save(null);
                };

                if (!ReminderDate.HasValue || !ReminderTime.HasValue)
                    RadMessageBox.Show(AppResources.ReminderText, MessageBoxButtons.YesNo, AppResources.ReminderMessageText, closedHandler: closedHandler);

                else
                {
                    var reminderDate = ReminderDate.Value.Date + ReminderTime.Value.TimeOfDay;

                    if (reminderDate > DateTime.Now)
                        save(reminderDate);

                    else
                        RadMessageBox.Show(AppResources.ReminderText, MessageBoxButtons.YesNo, AppResources.ReminderMessageText, closedHandler: closedHandler);
                }
            }

            else save(null);
        }

        private void GoBack()
        {
            if (App.RemoveBackOnNext && Id == Guid.Empty)
            {
                _navigationService.Navigate(new Uri(string.Format("/List/{0}", ListId), UriKind.Relative));
                App.RemoveBackEntry = true;
            }

            else _navigationService.GoBack();
            App.RemoveBackOnNext = false;
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Title);
        }
    }
}