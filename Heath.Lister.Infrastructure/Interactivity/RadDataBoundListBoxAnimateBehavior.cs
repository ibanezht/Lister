#region usings

using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Phone.Controls;
using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister.Infrastructure.Interactivity
{
    // Use QuarticEase

    public class RadDataBoundListBoxAnimateBehavior : Behavior<RadDataBoundListBox>
    {
        private Pivot _pivot;
        private int _pivotItemIndex;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnAssociatedObjectLoaded;
        }

        private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            _pivot = ElementTreeHelper.FindVisualAncestor<Pivot>(AssociatedObject);
            _pivotItemIndex = _pivot.Items.IndexOf(ElementTreeHelper.FindVisualAncestor<PivotItem>(AssociatedObject).DataContext);
            _pivot.ManipulationCompleted += ManipulationCompleted;
        }

        private void ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (_pivotItemIndex != _pivot.SelectedIndex)
                return;

            var fromRight = e.TotalManipulation.Translation.X <= 0;

            //var virtualizingStackPanel = (VirtualizingStackPanel)ElementTreeHelper.EnumVisualDescendants(AssociatedObject, obj => obj is VirtualizingStackPanel).SingleOrDefault();
        }
    }
}