﻿#region usings

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;
using Heath.Lister.Infrastructure.Extensions;
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

            AssociatedObject.ViewportItems
                .SelectMany(vi => ElementTreeHelper.EnumVisualDescendants(vi))
                .Where(p => GetAnimateLevel(p) > -1)
                .OfType<UIElement>()
                .ForEach(uie =>
                         {
                             var radMoveAnimation = new RadMoveAnimation();

                             radMoveAnimation.InitialDelay = TimeSpan.FromSeconds(GetAnimateLevel(uie) * 0.1 + 0.1);
                             radMoveAnimation.StartPoint = new Point(e.TotalManipulation.Translation.X <= 0 ? 100 : -100, 0);
                             radMoveAnimation.EndPoint = new Point(0, 0);
                             radMoveAnimation.Easing = new SineEase();

                             RadAnimationManager.Play(uie, radMoveAnimation);
                         }
                );
        }
    }
}