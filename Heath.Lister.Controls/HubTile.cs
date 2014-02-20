#region usings

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Heath.Lister.Controls
{
    [TemplateVisualState(Name = NormalState, GroupName = CommonStatesGroupName)]
    [TemplateVisualState(Name = FlippedState, GroupName = CommonStatesGroupName)]
    public class HubTile : Control
    {
        private const string CommonStatesGroupName = "CommonStates";
        private const string NormalState = "Normal";
        private const string FlippedState = "Flipped";
        private const string MessageLargePropertyName = "MessageLarge";
        private const string MessageSmallPropertyName = "MessageSmall";
        private const string StatePropertyName = "State";
        private const string TitlePropertyName = "Title";

        public static readonly DependencyProperty MessageLargeProperty =
            DependencyProperty.Register(
                MessageLargePropertyName,
                typeof (string),
                typeof (HubTile),
                null);

        public static readonly DependencyProperty MessageSmallProperty =
            DependencyProperty.Register(
                MessageSmallPropertyName,
                typeof (string),
                typeof (HubTile),
                null);

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(
                StatePropertyName,
                typeof (HubTileState),
                typeof (HubTile),
                new PropertyMetadata(HubTileState.Normal, OnStateChanged));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(TitlePropertyName, typeof (string), typeof (HubTile), null);

        internal int StallingCounter;

        public HubTile()
        {
            DefaultStyleKey = typeof (HubTile);

            Loaded += (sender, e) => HubTileService.InitializeReference(this);
            Unloaded += (sender, e) => HubTileService.FinalizeReference(this);
        }

        public string MessageLarge
        {
            get { return (string)GetValue(MessageLargeProperty); }
            set { SetValue(MessageLargeProperty, value); }
        }

        public string MessageSmall
        {
            get { return (string)GetValue(MessageSmallProperty); }
            set { SetValue(MessageSmallProperty, value); }
        }

        public HubTileState State
        {
            get { return (HubTileState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HubTile)d).UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            string state;

            switch (State)
            {
                case HubTileState.Normal:
                    state = NormalState;
                    break;

                case HubTileState.Flipped:
                    state = FlippedState;
                    break;

                default:
                    state = NormalState;
                    break;
            }

            VisualStateManager.GoToState(this, state, true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateVisualState();
        }
    }
}