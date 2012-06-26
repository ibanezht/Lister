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
using Heath.Lister.Infrastructure.ViewModels;
using Heath.Lister.Localization;
using Heath.Lister.ViewModels.Abstract;
using MediaColor = System.Windows.Media.Color;

#endregion

namespace Heath.Lister.ViewModels
{
    public class EditListViewModel : Abstract.ListViewModelBase, IHaveId, IPageViewModel
    {
        private const string PageNamePropertyName = "PageName";

        private readonly INavigationService _navigationService;

        private string _pageName;

        public EditListViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;

            ApplicationTitle = "LISTER";

            CancelCommand = new RelayCommand(Cancel);
            SaveCommand = new RelayCommand(Save, () => !string.IsNullOrWhiteSpace(Title));

            // TODO: Maybe make title overridable and put the Raise call in the setter?
            PropertyChanged += (sender, args) =>
                               {
                                   if (args.PropertyName == TitlePropertyName)
                                   {
                                       ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                                   }
                               };

            Colors = new ObservableCollection<ColorViewModel>();
        }

        public string ApplicationTitle { get; private set; }

        public ICommand CancelCommand { get; private set; }

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

        public ICommand SaveCommand { get; private set; }

        public IEnumerable<string> Suggestions
        {
            get
            {
                IEnumerable<string> retval;

                using (var data = new ListerData())
                    retval = data.GetListTitles();

                return retval;
            }
        }

        #region IPageViewModel Members

        public void Activate()
        {
            if (!Colors.Any())
            {
                using (var data = new ListerData())
                {
                    data.GetColors()
                        .Select(c => new ColorViewModel
                                     {
                                         Id = c.Id,
                                         Text = c.Text,
                                         Color = MediaColor.FromArgb(255, c.R, c.G, c.B)
                                     })
                        .ToList().ForEach(Colors.Add);
                }
            }

            var colorId = TombstoningHelper.Load<Guid>(ColorIdPropertyName);
            var remaining = TombstoningHelper.Load<int>(RemainingPropertyName);
            var title = TombstoningHelper.Load<string>(TitlePropertyName);

            if (Id != Guid.Empty)
            {
                PageName = string.Format("{0} {1}", AppResources.EditText, AppResources.ListText);

                using (var data = new ListerData())
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

            UpdatePin(isNavigationInitiator);
        }

        #endregion

        protected override void Loaded() {}

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
                    using (var data = new ListerData())
                        data.UpsertList(Id, ColorId, Title);
                };
            backgroundWorker.RunWorkerCompleted += (sender, args) => _navigationService.GoBack();
            backgroundWorker.RunWorkerAsync();
        }
    }
}