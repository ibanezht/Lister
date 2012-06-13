#region usings

using System;
using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister.Infrastructure
{
    public static class RateReminderHelper
    {
        private static readonly RadRateApplicationReminder _radRateApplicationReminder;

        static RateReminderHelper()
        {
            _radRateApplicationReminder = new RadRateApplicationReminder();
            _radRateApplicationReminder.RecurrencePerTimePeriod = new TimeSpan(7, 0, 0, 0);
            _radRateApplicationReminder.AllowUsersToSkipFurtherReminders = true;
        }

        public static void Notify()
        {
            _radRateApplicationReminder.Notify();
        }
    }
}