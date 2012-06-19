#region usings

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;
using Heath.Lister.Infrastructure.Interactivity;
using Heath.Lister.Infrastructure.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

#endregion

namespace Heath.Lister.Infrastructure.Extensions
{
    public static class PhoneApplicationPageExtensions
    {
        public static void AddApplicationBarIconButton(this PhoneApplicationPage page, Uri iconUri, string text, PropertyPath commandPath)
        {
            var button = new ApplicationBarIconButton();
            button.IconUri = iconUri;
            button.Text = text;

            page.ApplicationBar.Buttons.Add(button);

            var behaviors = Interaction.GetBehaviors(page);

            var behavior = new ApplicationBarMenuItemBehavior();
            behavior.ButtonText = text;

            var binding = new Binding();
            binding.Path = commandPath;
            binding.Source = page.DataContext;

            BindingOperations.SetBinding(behavior, ApplicationBarMenuItemBehavior.CommandProperty, binding);

            behaviors.Add(behavior);
        }

        public static void AddApplicationBarMenuItem(this PhoneApplicationPage page, string text, PropertyPath commandPath)
        {
            var menuItem = new ApplicationBarMenuItem();
            menuItem.Text = text;

            page.ApplicationBar.MenuItems.Add(menuItem);

            var behaviors = Interaction.GetBehaviors(page);

            var behavior = new ApplicationBarMenuItemBehavior();
            behavior.ButtonText = text;

            var binding = new Binding();
            binding.Path = commandPath;
            binding.Source = page.DataContext;

            BindingOperations.SetBinding(behavior, ApplicationBarMenuItemBehavior.CommandProperty, binding);

            behaviors.Add(behavior);
        }

        public static void ActivateViewModel(this PhoneApplicationPage page)
        {
            var haveId = page.DataContext as IHaveId;
            if (haveId != null)
                haveId.Id = Guid.Parse(page.NavigationContext.QueryString["Id"]);

            var haveListId = page.DataContext as IHaveListId;
            if (haveListId != null)
                haveListId.ListId = Guid.Parse(page.NavigationContext.QueryString["ListId"]);

            var viewModel = page.DataContext as IViewModel;
            if (viewModel != null)
                viewModel.Activate();
        }

        public static void DeactivateViewModel(this PhoneApplicationPage page, bool isNavigationInitiator)
        {
            var viewModel = page.DataContext as IViewModel;
            if (viewModel != null)
                viewModel.Deactivate(isNavigationInitiator);
        }
    }
}