﻿#region usings

using System;
using System.Windows;
using System.Windows.Navigation;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.Extensions;
using Heath.Lister.Localization;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister.Views
{
    public partial class HubView
    {
        public HubView()
        {
            InitializeComponent();

            InitializeApplicationBar();

            SetValue(RadTileAnimation.ContainerToAnimateProperty, hubItemsListBox);
        }

        private void InitializeApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            this.AddApplicationBarIconButton(new Uri("/Images/appbar.add.rest.png", UriKind.Relative), AppResources.AddText, new PropertyPath("AddCommand"));
            this.AddApplicationBarMenuItem(AppResources.AboutText, new PropertyPath("AboutCommand"));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.ActivateViewModel();
            App.ApplicationStartup = AppOpenState.None;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.DeactivateViewModel(e.IsNavigationInitiator);
            base.OnNavigatedFrom(e);
        }

        private void HubItemsListBoxItemTap(object sender, ListBoxItemTapEventArgs e)
        {
            SetValue(RadTileAnimation.ElementToDelayProperty, e.Item);
        }
    }
}