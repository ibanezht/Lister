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
        private bool _selectionChanged;

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
            _pivot.SelectionChanged += delegate { _selectionChanged = true; };
        }

        private void ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!_selectionChanged)
                return;

            _selectionChanged = false;

            if (_pivotItemIndex != _pivot.SelectedIndex)
                return;

            var fromRight = e.TotalManipulation.Translation.X <= 0;

            var visibleDescendants = AssociatedObject.ViewportItems
                .SelectMany(vi => ElementTreeHelper.EnumVisualDescendants(vi))
                .Where(p => GetAnimateLevel(p) > -1);

            foreach (UIElement visibleDescendant in visibleDescendants)
            {
                GetAnimation(visibleDescendant, fromRight).Begin();
            }
        }

        private static Storyboard GetAnimation(UIElement uiElement, bool fromRight)
        {
            var retval = new Storyboard();          

            double from = fromRight ? 80 : -80;

            var translateTransform = new TranslateTransform();
            translateTransform.X = from;

            uiElement.RenderTransform = translateTransform;

            var easingFunction = new SineEase();

            var doubleAnimation = new DoubleAnimation();
            doubleAnimation.To = 0;
            doubleAnimation.From = from;
            doubleAnimation.EasingFunction = easingFunction;

            retval.BeginTime = TimeSpan.FromSeconds(GetAnimateLevel(uiElement) * 0.1 + 0.1);
            retval.Duration = doubleAnimation.Duration = TimeSpan.FromSeconds(0.4);
            retval.Children.Add(doubleAnimation);

            Storyboard.SetTarget(doubleAnimation, translateTransform);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("X"));

            return retval;
        }
    }
}