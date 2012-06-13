#region usings

using System;
using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister.Infrastructure
{
    public static class TrialReminderHelper
    {
        private static readonly RadTrialApplicationReminder _radTrialApplicationReminder;

        private static bool _hasBeenNotified;

        static TrialReminderHelper()
        {
            _radTrialApplicationReminder = new RadTrialApplicationReminder();
            _radTrialApplicationReminder.AllowedTrialPeriod = new TimeSpan(30, 0, 0, 0);
            _radTrialApplicationReminder.FreePeriod = new TimeSpan(7, 0, 0, 0);
            _radTrialApplicationReminder.OccurrencePeriod = new TimeSpan(1, 0, 0, 0);
        }

        public static bool IsTrialExpired
        {
            get { return _radTrialApplicationReminder.IsTrialExpired; }
        }

        public static void Notify()
        {
            if (_hasBeenNotified)
                return;

            _radTrialApplicationReminder.Notify();
            _hasBeenNotified = true;
        }
    }
}