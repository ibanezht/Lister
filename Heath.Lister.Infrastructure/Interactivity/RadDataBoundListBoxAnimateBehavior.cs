#region usings

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister.Infrastructure.Interactivity
{
    // Use QuarticEase

    public class RadDataBoundListBoxAnimateBehavior : Behavior<RadDataBoundListBox>
    {
        private const string AnimateLevelPropertyName = "AnimateLevel";

        public static readonly DependencyProperty AnimateLevelProperty =
            DependencyProperty.RegisterAttached(
                AnimateLevelPropertyName,
                typeof(int),
                typeof(RadDataBoundListBoxAnimateBehavior),
                new PropertyMetadata(-1));

        private Pivot _pivot;
        private int _pivotItemIndex;

        public static int GetAnimateLevel(DependencyObject obj)
        {
            return (int)obj.GetValue(AnimateLevelProperty);
        }

        public static void SetAnimateLevel(DependencyObject obj, int value)
        {
            obj.SetValue(AnimateLevelProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnAssociatedObjectLoaded;
        }

        private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            _pivot = ElementTreeHelper.FindVisualAncestor<Pivot>(AssociatedObject);
            _pivotItemIndex = _pivot.Items.IndexOf(ElementTreeHelper.FindVisualAncestor<PivotItem>(AssociatedObject));
            _pivot.ManipulationCompleted += ManipulationCompleted;
        }

        private void ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (_pivotItemIndex != _pivot.SelectedIndex)
                return;

            var fromRight = e.TotalManipulation.Translation.X <= 0;

            foreach (var item in AssociatedObject.ViewportItems)
            {
                var des = ElementTreeHelper.EnumVisualDescendants(item, o => o is FrameworkElement);

                var ugh = des.FirstOrDefault();
            }

            //for (var index = firstVisibleItem; index <= firstVisibleItem + visibleItemCount; index++)
            //{
            //    // find all the item that have the AnimationLevel attached property set
            //    var lbi = list.ItemContainerGenerator.ContainerFromIndex(index);
            //    if (lbi == null)
            //        continue;

            //    vsp.Dispatcher.BeginInvoke(() =>
            //    {
            //        var animationTargets = lbi.Descendants().Where(p => GetAnimationLevel(p) > -1);
            //        foreach (FrameworkElement target in animationTargets)
            //        {
            //            // trigger the required animation
            //            GetAnimation(target, fromRight).Begin();
            //        }
            //    });
            //}
        }

        private static Storyboard GetAnimation(FrameworkElement element, bool fromRight)
        {
            double from = fromRight ? 80 : -80;

            var translateTransform = new TranslateTransform();
            translateTransform.X = from;
            element.RenderTransform = translateTransform;

            var retval = new Storyboard();
            retval.BeginTime = TimeSpan.FromSeconds(GetAnimateLevel(element) * 0.1 + 0.1);

            var doubleAnimation = new DoubleAnimation();
            doubleAnimation.To = 0;
            doubleAnimation.From = from;
            doubleAnimation.EasingFunction = new SineEase();

            retval.Duration = doubleAnimation.Duration = TimeSpan.FromSeconds(0.8);
            retval.Children.Add(doubleAnimation);

            Storyboard.SetTarget(doubleAnimation, translateTransform);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("X"));

            return retval;
        }
    }
}