#region usings

using System.Windows;
using Microsoft.Expression.Interactivity.Core;

#endregion

namespace Heath.Lister.Infrastructure.Interactivity
{
    public class ExtendedDataTrigger : DataTrigger
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            var element = AssociatedObject as FrameworkElement;
            if (element == null)
                return;

            element.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            var element = AssociatedObject as FrameworkElement;
            if (element != null)
                element.Loaded -= OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            EvaluateBindingChange(null);
        }
    }
}