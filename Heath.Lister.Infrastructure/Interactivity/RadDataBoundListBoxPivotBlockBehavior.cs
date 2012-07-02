#region usings

using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Phone.Controls;
using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister.Infrastructure.Interactivity
{
    public class RadDataBoundListBoxPivotBlockBehavior : Behavior<RadDataBoundListBox>
    {
        private readonly PivotBlocker _pivotBlocker;
        private Pivot _parentPivot;

        public RadDataBoundListBoxPivotBlockBehavior()
        {
            _pivotBlocker = new PivotBlocker();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnAssociatedObjectLoaded;
            AssociatedObject.IsCheckModeActiveChanged += OnAssociateObjectIsCheckModeActiveChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.IsCheckModeActiveChanged -= OnAssociateObjectIsCheckModeActiveChanged;
        }

        private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            EnsureParentPivot();

            if (_parentPivot == null)
                return;

            if (AssociatedObject.IsCheckModeActive)
            {
                _pivotBlocker.Start(_parentPivot);
            }
            else
            {
                _pivotBlocker.End();
            }
        }

        private void EnsureParentPivot()
        {
            if (_parentPivot != null)
                return;

            _parentPivot = ElementTreeHelper.FindVisualAncestor<Pivot>(AssociatedObject);
        }

        private void OnAssociateObjectIsCheckModeActiveChanged(object sender, IsCheckModeActiveChangedEventArgs e)
        {
            EnsureParentPivot();

            if (_parentPivot == null)
                return;

            if (AssociatedObject.IsCheckModeActive)
            {
                _pivotBlocker.Start(_parentPivot);
            }
            else
            {
                _pivotBlocker.End();
            }
        }
    }
}