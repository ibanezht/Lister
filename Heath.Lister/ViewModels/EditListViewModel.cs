#region usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.Extensions;
using Heath.Lister.Infrastructure.ViewModels;
using Heath.Lister.Localization;
using Heath.Lister.ViewModels.Abstract;
using MediaColor = System.Windows.Media.Color;

#endregion

namespace Heath.Lister.ViewModels
{
    public class EditListViewModel : ListViewModelBase, IHaveId, IPageViewModel
    {
        private const string PageNamePropertyName = "PageName";

        private readonly INavigationService _navigationService;

        private ICommand _cancelCommand;
        private string _pageName;
        private ICommand _saveCommand;

        public EditListViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;

            ApplicationTitle = "LISTER";

            Colors = new ObservableCollection<ColorViewModel>();
        }

        public string ApplicationTitle { get; private set; }

        public ICommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(Cancel)); }
        }

        public ObservableCollection<ColorViewModel> Colors { get; set; }

        public string PageName
        {
            get { return _pageName; }
            set
            {
                _pageName = value;
                RaisePropertyChanged(PageNamePropertyName);
            }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save, CanSave)); }
        }

        public IEnumerable<string> Suggestions
        {
            get
            {
                IEnumerable<string> retval;

                using (var data = new DataAccess())
                    retval = data.GetListTitles();

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
            }
        }

        #region IPageViewModel Members

        public void Activate()
        {
            if (!Colors.Any())
            {
                using (var data = new DataAccess())
                {
                    data.GetColors()
                        .Select(c => new ColorViewModel
                                     {
                                         Id = c.Id,
                                         Text = c.Text,
                                         Color = MediaColor.FromArgb(255, c.R, c.G, c.B)
                                     })
                        .ForEach(Colors.Add);
                }
            }

            var colorId = TombstoningHelper.Load<Guid>(ColorIdPropertyName);
            var remaining = TombstoningHelper.Load<int>(RemainingPropertyName);
            var title = TombstoningHelper.Load<string>(TitlePropertyName);

            if (Id != Guid.Empty)
            {
                PageName = string.Format("{0} {1}", AppResources.EditText, AppResources.ListText);

                using (var data = new DataAccess())
                {
                    var list = data.GetList(Id, true);

                    colorId = list.Color.Id;
                    CreatedDate = list.CreatedDate;
                    remaining = list.Items.Count(i => !i.Completed);
                    title = list.Title;
                }
            }

            else
                PageName = string.Format("{0} {1}", AppResources.AddText, AppResources.ListText);

            Color = Colors.SingleOrDefault(ci => ci.Id == colorId)
                    ?? Colors.SingleOrDefault(i => i.Color == ((SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"]).Color)
                    ?? Colors.First();

            Remaining = remaining;
            Title = title;
        }

        public void Deactivate(bool isNavigationInitiator)
        {
            if (isNavigationInitiator)
                TombstoningHelper.Clear();

            else
            {
                TombstoningHelper.Save(ColorIdPropertyName, ColorId);
                TombstoningHelper.Save(RemainingPropertyName, Remaining);
                TombstoningHelper.Save(TitlePropertyName, Title);
            }

            UpdatePin();
        }

        public void ViewReady() {}

        #endregion

        protected override void DeleteCompleted(object sender, RunWorkerCompletedEventArgs args) {}

        private void Cancel()
        {
            _navigationService.GoBack();
        }

        private void Save()
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork +=
                (sender, args) =>
                {
                    using (var data = new DataAccess())
                        data.UpsertList(Id, ColorId, Title);
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