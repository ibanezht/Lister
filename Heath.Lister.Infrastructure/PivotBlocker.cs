#region usings

using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister.Infrastructure
{
    public class PivotBlocker
    {
        private Pivot _target;

        public void Start(Pivot target)
        {
            _target = target;
            HidePivotHeaders();
            SubscribeEvents(target);
        }

        public void End()
        {
            if (_target == null)
                return;

            _target.IsHitTestVisible = true;
            ShowPivotHeaders();
            UnsubscribeEvents(_target);
            _target = null;
        }

        private void SubscribeEvents(Pivot target)
        {
            target.Unloaded -= TargetUnloaded;
            target.Unloaded += TargetUnloaded;
            target.ManipulationStarted -= TargetManipulationStarted;
            target.ManipulationStarted += TargetManipulationStarted;
        }

        private void UnsubscribeEvents(Pivot target)
        {
            Touch.FrameReported -= TouchFrameReported;
            target.Unloaded -= TargetUnloaded;
            target.ManipulationStarted -= TargetManipulationStarted;
        }

        private void HidePivotHeaders()
        {
            var currentItem = (PivotItem)_target.SelectedItem;

            foreach (PivotItem item in _target.Items)
            {
                if (!ReferenceEquals(item, currentItem))
                {
                    //item.Tag = item.Header;
                    //item.Header = null;
                    HideAnimate(item);
                }
            }
        }

        private void ShowPivotHeaders()
        {
            var currentItem = (PivotItem)_target.SelectedItem;

            foreach (PivotItem item in _target.Items)
            {
                if (!ReferenceEquals(item, currentItem) && item.Tag != null)
                {
                    //item.Header = item.Tag;
                    //item.Tag = null;
                    ShowAnimate(item);
                }
            }
        }

        private static void HideAnimate(UIElement uiElement)
        {
            var radMoveAndFadeAnimation = new RadMoveAndFadeAnimation();

            radMoveAndFadeAnimation.FadeAnimation.StartOpacity = 1.0;
            radMoveAndFadeAnimation.FadeAnimation.EndOpacity = 0.0;
            radMoveAndFadeAnimation.MoveAnimation.StartPoint = new Point(0, 0);
            radMoveAndFadeAnimation.MoveAnimation.EndPoint = new Point(75, 0);
            radMoveAndFadeAnimation.Easing = new SineEase();

            RadAnimationManager.Play(uiElement, radMoveAndFadeAnimation);
        }

        private static void ShowAnimate(UIElement uiElement)
        {
            var radMoveAndFadeAnimation = new RadMoveAndFadeAnimation();

            radMoveAndFadeAnimation.FadeAnimation.StartOpacity = 0.0;
            radMoveAndFadeAnimation.FadeAnimation.EndOpacity = 1.0;
            radMoveAndFadeAnimation.MoveAnimation.StartPoint = new Point(75, 0);
            radMoveAndFadeAnimation.MoveAnimation.EndPoint = new Point(0, 0);
            radMoveAndFadeAnimation.Easing = new SineEase();

            RadAnimationManager.Play(uiElement, radMoveAndFadeAnimation);
        }

        private void TargetManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Touch.FrameReported -= TouchFrameReported;
            Touch.FrameReported += TouchFrameReported;
            _target.IsHitTestVisible = false;
        }

        private void TargetUnloaded(object sender, RoutedEventArgs e)
        {
            End();
        }

        private void TouchFrameReported(object sender, TouchFrameEventArgs e)
        {
            var touchPoints = e.GetTouchPoints(_target);

            if (touchPoints.Count == 0)
            {
                Touch.FrameReported -= TouchFrameReported;
                _target.IsHitTestVisible = true;
            }

            if (touchPoints.Count == 1 && touchPoints[0].Action == TouchAction.Up)
            {
                Touch.FrameReported -= TouchFrameReported;
                _target.IsHitTestVisible = true;
            }
        }
    }
}