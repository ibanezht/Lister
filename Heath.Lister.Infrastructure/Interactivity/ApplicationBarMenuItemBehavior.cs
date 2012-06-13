#region usings

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

#endregion

namespace Heath.Lister.Infrastructure.Interactivity
{
    public class ApplicationBarMenuItemBehavior : Behavior<PhoneApplicationPage>
    {
        private const string CommandBindingPropertyName = "CommandBinding";
        private const string CommandParameterPropertyName = "CommandParameter";

        public static readonly DependencyProperty CommandBindingProperty =
            DependencyProperty.Register(
                CommandBindingPropertyName,
                typeof(ICommand),
                typeof(ApplicationBarMenuItemBehavior),
                new PropertyMetadata(OnCommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                CommandParameterPropertyName,
                typeof(object),
                typeof(ApplicationBarMenuItemBehavior),
                new PropertyMetadata(OnCommandChanged));

        private ClickCommandBinding _binding;

        public string ButtonText { get; set; }

        public ICommand CommandBinding
        {
            get { return (ICommand)GetValue(CommandBindingProperty); }
            set { SetValue(CommandBindingProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            CreateBinding();
        }

        private static void OnCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var applicationBarIconButtonBehavior = (ApplicationBarMenuItemBehavior)sender;
            if (applicationBarIconButtonBehavior != null)
                applicationBarIconButtonBehavior.CreateBinding();
        }

        private void CreateBinding()
        {
            if (AssociatedObject == null || CommandBinding == null)
                return;

            var buttons = AssociatedObject.ApplicationBar.Buttons
                .Cast<IApplicationBarMenuItem>();

            var menuItems = AssociatedObject.ApplicationBar.MenuItems
                .Cast<IApplicationBarMenuItem>();

            var applicationBarMenuItem = buttons.Concat(menuItems)
                .FirstOrDefault(b => b != null && b.Text == ButtonText);

            _binding = new ClickCommandBinding(applicationBarMenuItem, CommandBinding, () => CommandParameter);
        }

        #region Nested type: ClickCommandBinding

        private class ClickCommandBinding
        {
            private readonly IApplicationBarMenuItem _applicationBarMenuItem;
            private readonly ICommand _command;
            private readonly Func<object> _parameterGetter;

            public ClickCommandBinding(IApplicationBarMenuItem applicationBarMenuItem, ICommand command, Func<object> parameterGetter)
            {
                _applicationBarMenuItem = applicationBarMenuItem;
                _command = command;
                _parameterGetter = parameterGetter;
                _applicationBarMenuItem.IsEnabled = _command.CanExecute(parameterGetter());

                _command.CanExecuteChanged += (s, e) => { _applicationBarMenuItem.IsEnabled = _command.CanExecute(_parameterGetter()); };
                _applicationBarMenuItem.Click += (s, e) => _command.Execute(_parameterGetter());
            }
        }

        #endregion
    }
}