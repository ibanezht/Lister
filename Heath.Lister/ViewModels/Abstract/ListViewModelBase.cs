#region usings

using System;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Heath.Lister.Configuration;
using Heath.Lister.Infrastructure;
using Heath.Lister.Localization;
using Heath.Lister.Views;
using Telerik.Windows.Controls;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

#endregion

namespace Heath.Lister.ViewModels.Abstract
{
    public abstract class ListViewModelBase : ViewModelBase
    {
        protected const string ColorPropertyName = "Color";
        protected const string ColorIdPropertyName = "ColorId";
        protected const string CreatedDatePropertyName = "CreatedDate";
        protected const string IdPropertyName = "Id";
        protected const string RemainingPropertyName = "Remaining";
        protected const string TitlePropertyName = "Title";

        private readonly INavigationService _navigationService;

        private ColorViewModel _color;
        private Guid _colorId;
        private DateTime _createdDate;
        private ICommand _deleteCommand;
        private ICommand _editCommand;
        private Guid _id;
        private ICommand _pinCommand;
        private int _remaining;
        private string _title;

        protected ListViewModelBase(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public ColorViewModel Color
        {
            get { return _color; }
            set
            {
                _color = value;

                if (_color != null)
                    ColorId = _color.Id;

                RaisePropertyChanged(ColorPropertyName);
            }
        }

        public Guid ColorId
        {
            get { return _colorId; }
            set
            {
                _colorId = value;
                RaisePropertyChanged(ColorIdPropertyName);
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
                ((RelayCommand)PinCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand PinCommand
        {
            get { return _pinCommand ?? (_pinCommand = new RelayCommand(Pin, CanPin)); }
        }

        public int Remaining
        {
            get { return _remaining; }
            set
            {
                _remaining = value;
                RaisePropertyChanged(RemainingPropertyName);
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

        public event EventHandler ReminderCompleted;

        public event EventHandler ReminderRequested;

        private void Delete()
        {
            Action<MessageBoxClosedEventArgs> closedHandler =
                e =>
                {
                    if (e.Result != DialogResult.OK)
                        return;

                    var backgroundWorker = new BackgroundWorker();
                    backgroundWorker.DoWork +=
                        (sender, args) =>
                        {
                            using (var data = new DataAccess())
                                data.DeleteList(Id);

                            var shellTile = LiveTileHelper.GetTile(UriMappings.Instance.MapUri(new Uri(string.Format("/List/{0}", Id), UriKind.Relative)));

                            if (shellTile != null)
                                shellTile.Delete();
                        };
                    backgroundWorker.RunWorkerCompleted += DeleteCompleted;
                    backgroundWorker.RunWorkerAsync();
                };

            RadMessageBox.Show(AppResources.DeleteText, MessageBoxButtons.YesNo, AppResources.DeleteListMessage, closedHandler: closedHandler);
        }

        protected abstract void DeleteCompleted(object sender, RunWorkerCompletedEventArgs args);

        protected void UpdatePin()
        {
            var uri = UriMappings.Instance.MapUri(new Uri(string.Format("/List/{0}", Id), UriKind.Relative));

            var shellTile = LiveTileHelper.GetTile(uri);
            if (shellTile == null)
                return;

            var hubItem = new HubItemView();

            hubItem.DataContext = this;
            hubItem.UpdateLayout();

            LiveTileHelper.UpdateTile(shellTile, new RadExtendedTileData { VisualElement = hubItem });
        }

        private void Edit()
        {
            _navigationService.Navigate(new Uri(string.Format("/EditList/{0}", Id), UriKind.Relative));
        }

        private void Pin()
        {
            var uri = UriMappings.Instance.MapUri(new Uri(string.Format("/List/{0}", Id), UriKind.Relative));

            var hubItem = new HubItemView();

            hubItem.DataContext = this;
            hubItem.UpdateLayout();

            LiveTileHelper.CreateOrUpdateTile(new RadExtendedTileData { VisualElement = hubItem }, uri);
        }

        private bool CanPin()
        {
            return LiveTileHelper.GetTile(UriMappings.Instance.MapUri(new Uri(string.Format("/List/{0}", Id), UriKind.Relative))) == null;
        }

        protected virtual void OnReminderCompleted(EventArgs e)
        {
            if (ReminderCompleted != null)
                ReminderCompleted(this, e);
        }

        protected virtual void OnReminderRequested(EventArgs e)
        {
            if (ReminderRequested != null)
                ReminderRequested(this, e);
        }
    }
}