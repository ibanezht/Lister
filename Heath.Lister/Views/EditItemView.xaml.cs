#region usings

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.Extensions;
using Heath.Lister.Localization;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister.Views
{
    public partial class EditItemView
    {
        private bool _newInstance;

        public EditItemView()
        {
            InitializeComponent();

            InitializeApplicationBar();

            Hide(reminderPanel);

            _newInstance = true;
        }

        private void InitializeApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            this.AddApplicationBarIconButton(new Uri("/Images/Save.png", UriKind.Relative), AppResources.SaveText, new PropertyPath("SaveCommand"));
            this.AddApplicationBarIconButton(new Uri("/Images/SaveNext.png", UriKind.Relative), AppResources.SaveNextText, new PropertyPath("SaveNextCommand"));
            this.AddApplicationBarIconButton(new Uri("/Images/appbar.cancel.rest.png", UriKind.Relative), AppResources.CancelText, new PropertyPath("CancelCommand"));
        }

        private void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.AppOpenState != AppOpenState.Activated || _newInstance)
                this.ActivateViewModel();

            App.AppOpenState = AppOpenState.None;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.DeactivateViewModel(e.IsNavigationInitiator);
            base.OnNavigatedFrom(e);

            _newInstance = false;
        }

        private void ReminderToggleSwitchCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.NewState)
            {
                Show(reminderPanel);
            }
            else
            {
                Hide(reminderPanel);
            }
        }

        private static void Show(StackPanel stackPanel)
        {
            //var height = stackPanel.Children.Sum(c => c.DesiredSize.Height);

            var radResizeHeightAnimation = new RadResizeHeightAnimation();
            radResizeHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));
            radResizeHeightAnimation.StartHeight = 0;
            radResizeHeightAnimation.EndHeight = null;
            RadAnimationManager.Play(stackPanel, radResizeHeightAnimation);
        }

        private static void Hide(StackPanel stackPanel)
        {
            var radResizeHeightAnimation = new RadResizeHeightAnimation();
            radResizeHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));
            radResizeHeightAnimation.StartHeight = stackPanel.ActualHeight;
            radResizeHeightAnimation.EndHeight = 0;
            RadAnimationManager.Play(stackPanel, radResizeHeightAnimation);
        }
    }
}