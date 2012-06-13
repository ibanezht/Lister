#region usings

using System;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Navigation;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.Extensions;
using Heath.Lister.Localization;
using Heath.Lister.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using PivotBlocker = Heath.Lister.Infrastructure.PivotBlocker;

#endregion

namespace Heath.Lister.Views
{
    public partial class ListView
    {
        private readonly ListViewModel _listView;
        private readonly PivotBlocker _pivotBlocker = new PivotBlocker();

        public ListView()
        {
            InitializeComponent();

            _listView = (ListViewModel)DataContext;
            _listView.IsCheckModeActiveChanged += IsCheckModeActiveChanged;

            InitializeDefaultApplicationBar();

            listPivot.LoadedPivotItem += (sender, args) => AnimateSelectedListBox(args.Item);

            Loaded += (sender, args) =>
                      {                         
                          AnimateSelectedListBox();

                          RateReminderHelper.Notify();
                          TrialReminderHelper.Notify();
                      };
        }

        private void IsCheckModeActiveChanged(object sender, IsCheckModeActiveChangedEventArgs e)
        {
            if (!e.CheckBoxesVisible)
            {
                _pivotBlocker.End();
                InitializeDefaultApplicationBar();
            }
            else
            {
                _pivotBlocker.Start(listPivot);
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

        private void AnimateSelectedListBox(PivotItem pivotItem = null)
        {
            var listBox = GetSelectedPivotItemListBox(pivotItem);

            SetValue(RadTileAnimation.ContainerToAnimateProperty, listBox);
        }

        private RadDataBoundListBox GetSelectedPivotItemListBox(PivotItem pivotItem = null)
        {
            DependencyObject dependencyObject;

            if (pivotItem == null)
                dependencyObject = listPivot;
            else
                dependencyObject = pivotItem;

            return ElementTreeHelper.EnumVisualDescendants(dependencyObject, obj => obj is RadDataBoundListBox)
                .Cast<RadDataBoundListBox>()
                .FirstOrDefault(lb => lb.DataContext == listPivot.SelectedItem);
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
    }
}