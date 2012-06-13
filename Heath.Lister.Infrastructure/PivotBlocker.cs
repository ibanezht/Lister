#region usings

using System.Windows;
using System.Windows.Input;
using Heath.Lister.Infrastructure.ViewModels;
using Microsoft.Phone.Controls;

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
            target.Unloaded -= OnTargetPivotUnloaded;
            target.Unloaded += OnTargetPivotUnloaded;
            target.ManipulationStarted -= OnTargetPivotManipulationStarted;
            target.ManipulationStarted += OnTargetPivotManipulationStarted;
        }

        private void HidePivotHeaders()
        {
            var currentItem = _target.SelectedItem as IHaveHeader;

            foreach (IHaveHeader item in _target.Items)
            {
                if (!ReferenceEquals(item, currentItem))
                {
                    item.Tag = item.Header;
                    item.Header = null;
                }
            }
        }

        private void ShowPivotHeaders()
        {
            var currentItem = _target.SelectedItem as IHaveHeader;

            foreach (IHaveHeader item in _target.Items)
            {
                if (!ReferenceEquals(item, currentItem) && item.Tag != null)
                {
                    item.Header = item.Tag;
                    item.Tag = null;
                }
            }
        }

        private void UnsubscribeEvents(Pivot target)
        {
            Touch.FrameReported -= OnTouchFrameReported;
            target.Unloaded -= OnTargetPivotUnloaded;
            target.ManipulationStarted -= OnTargetPivotManipulationStarted;
        }

        private void OnTargetPivotManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Touch.FrameReported -= OnTouchFrameReported;
            Touch.FrameReported += OnTouchFrameReported;
            _target.IsHitTestVisible = false;
        }

        private void OnTargetPivotUnloaded(object sender, RoutedEventArgs e)
        {
            End();
        }

        private void OnTouchFrameReported(object sender, TouchFrameEventArgs e)
        {
            var touchPoints = e.GetTouchPoints(_target);

            if (touchPoints.Count == 0)
            {
                Touch.FrameReported -= OnTouchFrameReported;
                _target.IsHitTestVisible = true;
            }

            if (touchPoints.Count == 1 && touchPoints[0].Action == TouchAction.Up)
            {
                Touch.FrameReported -= OnTouchFrameReported;
                _target.IsHitTestVisible = true;
            }
        }
    }
}