#region usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.Models;
using Heath.Lister.Infrastructure.ViewModels;
using Heath.Lister.Localization;
using Heath.Lister.ViewModels.Abstract;
using Color = System.Windows.Media.Color;

#endregion

namespace Heath.Lister.ViewModels
{
    public class EditItemViewModel : ItemViewModelBase, IHaveListId, IPageViewModel
    {
        private const string PageNamePropertyName = "PageName";

        private readonly INavigationService _navigationService;
        private ICommand _cancelCommand;
        private ICommand _clearDateCommand;
        private ICommand _clearTimeCommand;

        private string _pageName;
        private ICommand _saveCommand;

        public EditItemViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;

            ApplicationTitle = "LISTER";

            // TODO: Maybe make title overridable and put the Raise call in the setter?
            PropertyChanged += (sender, args) =>
                               {
                                   if (args.PropertyName == TitlePropertyName)
                                   {
                                       ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                                   }
                               };
        }


        public string ApplicationTitle { get; private set; }

        public ICommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(Cancel)); }
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

        public IEnumerable<string> Suggestions
        {
            get
            {
                IEnumerable<string> retval;

                using (var data = new ListerData())
                    retval = data.GetItemTitles();

                return retval;
            }
        }

        public ICommand ClearDateCommand
        {
            get { return _clearDateCommand ?? (_clearDateCommand = new RelayCommand(ClearDate)); }
        }

        public ICommand ClearTimeCommand
        {
            get { return _clearTimeCommand ?? (_clearTimeCommand = new RelayCommand(ClearTime)); }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save, CanSave)); }
        }

        #region IPageViewModel Members

        public void Activate()
        {
            var completed = TombstoningHelper.Load<bool>(CompletedPropertyName);
            var dueDate = TombstoningHelper.Load<DateTime?>(DueDatePropertyName);
            var dueTime = TombstoningHelper.Load<DateTime?>(DueTimePropertyName);
            var notes = TombstoningHelper.Load<string>(NotesPropertyName);
            var priority = TombstoningHelper.Load<Priority>(PriorityPropertyName);
            var title = TombstoningHelper.Load<string>(TitlePropertyName);

            if (Id != Guid.Empty)
            {
                PageName = string.Format("{0} {1}", AppResources.EditText, AppResources.ItemText);

                using (var data = new ListerData())
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

                using (var data = new ListerData())
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

        #endregion

        public override void Loaded() {}

        protected override void CompleteCompleted(object sender, RunWorkerCompletedEventArgs args) {}

        protected override void DeleteCompleted(object sender, RunWorkerCompletedEventArgs args) {}

        protected override void IncompleteCompleted(object sender, RunWorkerCompletedEventArgs args) {}

        private void Cancel()
        {
            _navigationService.GoBack();
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
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork +=
                (sender, args) =>
                {
                    using (var data = new ListerData())
                        data.UpsertItem(Id, ListId, Completed, DueDate, DueTime, Notes, Priority, Title);
                };
            backgroundWorker.RunWorkerCompleted += (sender, args) => _navigationService.GoBack();
            backgroundWorker.RunWorkerAsync();
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Title);
        }
    }
}