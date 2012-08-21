#region usings

using System;
using System.Linq;
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
        private readonly RadResizeHeightAnimation _radResizeHeightAnimation;

        private bool _newInstance;

        public EditItemView()
        {
            InitializeComponent();

            InitializeApplicationBar();

            _radResizeHeightAnimation = new RadResizeHeightAnimation();
            _radResizeHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(150));

            Hide(reminderPanel);

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

        private void Show(StackPanel stackPanel)
        {
            var height = stackPanel.DesiredSize.Height +
                         stackPanel.Children.Sum(c => c.DesiredSize.Height);

            _radResizeHeightAnimation.StartHeight = 0;
            _radResizeHeightAnimation.EndHeight = height;
            RadAnimationManager.Play(stackPanel, _radResizeHeightAnimation);
        }

        private void Hide(StackPanel stackPanel)
        {
            _radResizeHeightAnimation.StartHeight = stackPanel.ActualHeight;
            _radResizeHeightAnimation.EndHeight = 0;
            RadAnimationManager.Play(stackPanel, _radResizeHeightAnimation);
        }
    }
}