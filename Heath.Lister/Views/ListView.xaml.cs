#region usings

using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.Extensions;
using Heath.Lister.Localization;
using Heath.Lister.ViewModels;
using Microsoft.Advertising;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister.Views
{
    public partial class ListView
    {
        public ListView()
        {
            InitializeComponent();

            InitializeDefaultApplicationBar();

            AnimateSelectedListBox();

            listPivot.LoadedPivotItem += (sender, args) => AnimateSelectedListBox();

            Loaded += (sender, args) => this.ViewReady();
        }

        private void IsCheckModeActiveChanged(object sender, IsCheckModeActiveChangedEventArgs e)
        {
            if (!e.CheckBoxesVisible)
            {
                InitializeDefaultApplicationBar();
            }
            else
            {
                InitializeSelectApplicationBar();
            }
        }

        private void InitializeDefaultApplicationBar()
        {
            var behaviors = Interaction.GetBehaviors(this);
            behaviors.Clear();

            if (ApplicationBar == null)
                ApplicationBar = new ApplicationBar();

            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();

            this.AddApplicationBarIconButton(new Uri("/Images/appbar.add.rest.png", UriKind.Relative), AppResources.AddText, new PropertyPath("AddCommand"));
            this.AddApplicationBarIconButton(new Uri("/Images/appbar.select.rest.png", UriKind.Relative), AppResources.SelectText, new PropertyPath("SelectCommand"));
            this.AddApplicationBarIconButton(new Uri("/Images/appbar.edit.rest.png", UriKind.Relative), AppResources.EditText, new PropertyPath("EditCommand"));
            this.AddApplicationBarIconButton(new Uri("/Images/appbar.delete.rest.png", UriKind.Relative), AppResources.DeleteText, new PropertyPath("DeleteCommand"));
            this.AddApplicationBarMenuItem(AppResources.CompleteAllText, new PropertyPath("CompleteAllCommand"));
            this.AddApplicationBarMenuItem(AppResources.PinToStartText, new PropertyPath("PinCommand"));
            this.AddApplicationBarMenuItem(AppResources.ShareText, new PropertyPath("ShareCommand"));
        }

        private void InitializeSelectApplicationBar()
        {
            var behaviors = Interaction.GetBehaviors(this);
            behaviors.Clear();

            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();

            this.AddApplicationBarIconButton(new Uri("/Images/out.png", UriKind.Relative), AppResources.CompleteText, new PropertyPath("CompleteSelectedCommand"));
            this.AddApplicationBarIconButton(new Uri("/Images/appbar.delete.rest.png", UriKind.Relative), AppResources.DeleteText, new PropertyPath("DeleteSelectedCommand"));
        }

        private void AnimateSelectedListBox()
        {
            RadDataBoundListBox listBox;

            switch (listPivot.SelectedIndex)
            {
                case 0:
                    listBox = allListItemsListBox;
                    break;

                case 1:
                    listBox = todayListItemsListBox;
                    break;

                case 2:
                    listBox = overdueListItemsListBox;
                    break;

                default:
                    listBox = allListItemsListBox;
                    break;
            }

            SetValue(RadTileAnimation.ContainerToAnimateProperty, listBox);
        }

        private void ListItemsRadDataBoundListBoxItemTap(object sender, ListBoxItemTapEventArgs e)
        {
            SetValue(RadTileAnimation.ElementToDelayProperty, e.Item);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.ActivateViewModel();

            Messenger.Default.Register<NotificationMessage<ListViewModel>>(
                this, nm =>
                      {
                          if (nm.Notification == "SortCompleted")
                          {
                              ElementTreeHelper.FindVisualDescendant<RadPickerBox>(this).IsPopupOpen = false;
                          }
                      });

            App.ApplicationStartup = AppOpenState.None;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.DeactivateViewModel(e.IsNavigationInitiator);
            Messenger.Default.Unregister<NotificationMessage<ListViewModel>>(this);
            base.OnNavigatedFrom(e);
        }

        private void AdControlErrorOccurred(object sender, AdErrorEventArgs e)
        {
            DispatcherHelper.UIDispatcher.BeginInvoke(() => { adControl.Visibility = Visibility.Collapsed; });
        }
    }
}