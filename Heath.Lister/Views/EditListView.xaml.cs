#region usings

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.Extensions;
using Heath.Lister.Localization;
using Microsoft.Phone.Shell;

#endregion

namespace Heath.Lister.Views
{
    public partial class EditListView
    {
        private bool _newInstance;

        public EditListView()
        {
            InitializeComponent();

            InitializeApplicationBar();

            _newInstance = true;
        }

        private void InitializeApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            this.AddApplicationBarIconButton(new Uri("/Images/appbar.save.rest.png", UriKind.Relative), AppResources.SaveText, new PropertyPath("SaveCommand"));
            this.AddApplicationBarIconButton(new Uri("/Images/appbar.cancel.rest.png", UriKind.Relative), AppResources.CancelText, new PropertyPath("CancelCommand"));
        }

        private void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
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