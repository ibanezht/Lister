#region usings

using System.Reflection;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister.Infrastructure.Interactivity
{
    public class RadDataBoundListBoxTapBehavior : Behavior<RadDataBoundListBox>
    {
        private const string MethodNamePropertyName = "MethodName";
        private const string TargetPropertyName = "Target";

        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register(
                MethodNamePropertyName,
                typeof(string),
                typeof(RadDataBoundListBoxTapBehavior),
                new PropertyMetadata(TargetChanged));

        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register(
                TargetPropertyName,
                typeof(object),
                typeof(RadDataBoundListBoxTapBehavior),
                new PropertyMetadata(TargetChanged));

        private bool _canTap = true;
        private MethodInfo _methodInfo;

        public object TargetObject
        {
            get { return GetValue(TargetObjectProperty); }
            set { SetValue(TargetObjectProperty, value); }
        }

        public string MethodName
        {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.IsCheckModeActiveChanged += IsCheckModeActiveChanged;
            AssociatedObject.IsCheckModeActiveChanging += IsCheckModeActiveChanging;
            AssociatedObject.ItemTap += ItemTap;
        }

        private static void TargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var radDataBoundListBoxTapBehavior = sender as RadDataBoundListBoxTapBehavior;
            if (radDataBoundListBoxTapBehavior != null)
                radDataBoundListBoxTapBehavior.EnsureMethodInfo();
        }

        private void EnsureMethodInfo()
        {
            if (TargetObject == null || MethodName == null)
                return;

            _methodInfo = TargetObject.GetType().GetMethod(MethodName);
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
                _methodInfo.Invoke(TargetObject, new[] { e });
            }
        }
    }
}