#region usings

using System;
using System.Windows;
using System.Windows.Navigation;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.Extensions;
using Heath.Lister.Localization;
using Heath.Lister.ViewModels;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister.Views
{
    public partial class ItemDetailsView
    {
        private readonly ItemDetailsViewModel _itemDetails;

        private bool _newInstance;

        public ItemDetailsView()
        {
            InitializeComponent();

            _itemDetails = (ItemDetailsViewModel)DataContext;
            _itemDetails.PropertyChanged += (sender, args) =>
                                            {
                                                if (args.PropertyName == "Completed")
                                                    InitializeApplicationBar();
                                            };

            _itemDetails.ReminderRequested += (sender, args) =>
                                              {
                                                  reminderView.IsOpen = true;
                                              };

            Loaded += (sender, args) => InitializeApplicationBar();

            _newInstance = true;
        }

        private void InitializeApplicationBar()
        {
            if (_itemDetails.Completed)
            {
                InitializeIncompleteApplicationBar();
            }
            else
            {
                InitializeCompleteApplicationBar();
            }
        }

        private void InitializeCompleteApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            this.AddApplicationBarIconButton(new Uri("/Images/out.png", UriKind.Relative), AppResources.CompleteText, new PropertyPath("CompleteCommand"));
            this.AddApplicationBarIconButton(new Uri("/Images/appbar.edit.rest.png", UriKind.Relative), AppResources.EditText, new PropertyPath("EditCommand"));
            this.AddApplicationBarIconButton(new Uri("/Images/appbar.delete.rest.png", UriKind.Relative), AppResources.DeleteText, new PropertyPath("DeleteCommand"));
            this.AddApplicationBarMenuItem(AppResources.ItemReminderText, new PropertyPath("ReminderCommand"));
            this.AddApplicationBarMenuItem(AppResources.PinToStartText, new PropertyPath("PinCommand"));
        }

        private void InitializeIncompleteApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            this.AddApplicationBarIconButton(new Uri("/Images/in.png", UriKind.Relative), AppResources.IncompleteText, new PropertyPath("IncompleteCommand"));
            this.AddApplicationBarIconButton(new Uri("/Images/appbar.edit.rest.png", UriKind.Relative), AppResources.EditText, new PropertyPath("EditCommand"));
            this.AddApplicationBarIconButton(new Uri("/Images/appbar.delete.rest.png", UriKind.Relative), AppResources.DeleteText, new PropertyPath("DeleteCommand"));
            this.AddApplicationBarMenuItem(AppResources.ItemReminderText, new PropertyPath("ReminderCommand"));
            this.AddApplicationBarMenuItem(AppResources.PinToStartText, new PropertyPath("PinCommand"));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.ApplicationStartup != AppOpenState.Activated || _newInstance)
                this.ActivateViewModel();

            App.ApplicationStartup = AppOpenState.None;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.DeactivateViewModel(e.IsNavigationInitiator);
            base.OnNavigatedFrom(e);
            _newInstance = false;
        }
    }
}