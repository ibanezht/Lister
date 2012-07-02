#region usings

using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;

#endregion

namespace Heath.Lister.Infrastructure
{
    public class PivotBlocker
    {
        private Pivot _target;

        public Pivot Target
        {
            get { return _target; }
        }

        public void Start(Pivot target)
        {
            if (_target != null)
                throw new InvalidOperationException("Blocking already started. Please end the blocking operation for the current target before starting a new one.");

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
            target.Unloaded -= Unloaded;
            target.Unloaded += Unloaded;
            target.ManipulationStarted -= ManipulationStarted;
            target.ManipulationStarted += ManipulationStarted;
        }

        private void HidePivotHeaders()
        {
            var currentItem = _target.SelectedItem as PivotItem;

            foreach (PivotItem item in _target.Items)
            {
                if (ReferenceEquals(item, currentItem))
                    continue;

                item.Tag = item.Header;
                item.Header = null;
            }
        }

        private void ShowPivotHeaders()
        {
            var currentItem = _target.SelectedItem as PivotItem;

            foreach (PivotItem item in _target.Items)
            {
                if (ReferenceEquals(item, currentItem) || item.Tag == null)
                    continue;

                item.Header = item.Tag;
                item.Tag = null;
            }
        }

        private void UnsubscribeEvents(Pivot target)
        {
            Touch.FrameReported -= FrameReported;
            target.Unloaded -= Unloaded;
            target.ManipulationStarted -= ManipulationStarted;
        }

        private void ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Touch.FrameReported -= FrameReported;
            Touch.FrameReported += FrameReported;
            _target.IsHitTestVisible = false;
        }

        private void Unloaded(object sender, RoutedEventArgs e)
        {
            End();
        }

        private void FrameReported(object sender, TouchFrameEventArgs e)
        {
            var touchPoints = e.GetTouchPoints(_target);

            if (touchPoints.Count == 0)
            {
                Touch.FrameReported -= FrameReported;
                _target.IsHitTestVisible = true;
            }

            if (touchPoints.Count == 1 && touchPoints[0].Action == TouchAction.Up)
            {
                Touch.FrameReported -= FrameReported;
                _target.IsHitTestVisible = true;
            }
        }
    }
}