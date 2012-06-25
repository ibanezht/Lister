#region usings

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister.Infrastructure.Interactivity
{
    public class RadDataBoundListBoxTapBehavior : Behavior<RadDataBoundListBox>
    {
        private const string CommandPropertyName = "Command";

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                CommandPropertyName,
                typeof(ICommand),
                typeof(RadDataBoundListBoxTapBehavior),
                null);

        private bool _canTap = true;

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.IsCheckModeActiveChanged += IsCheckModeActiveChanged;
            AssociatedObject.IsCheckModeActiveChanging += IsCheckModeActiveChanging;
            AssociatedObject.ItemTap += ItemTap;
        }

        private void IsCheckModeActiveChanged(object sender, IsCheckModeActiveChangedEventArgs e)
        {
            _canTap = !e.CheckBoxesVisible;
        }

        private void IsCheckModeActiveChanging(object sender, IsCheckModeActiveChangingEventArgs e)
        {
            _canTap = false;
        }

        private void ItemTap(object sender, ListBoxItemTapEventArgs e)
        {
            if (_canTap)
            {
                Command.Execute(e);
            }
        }
    }
}