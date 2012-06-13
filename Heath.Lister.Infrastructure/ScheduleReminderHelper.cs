#region usings

using System;
using Microsoft.Phone.Scheduler;

#endregion

namespace Heath.Lister.Infrastructure
{
    public static class ScheduleReminderHelper
    {
        public static void AddReminder(string name, Uri uri, string title, string content, DateTime beginTime)
        {
            RemoveReminder(name);

            var reminder = new Reminder(name);

            reminder.NavigationUri = uri;
            reminder.Title = title;
            reminder.Content = content;
            reminder.BeginTime = beginTime;
            reminder.RecurrenceType = RecurrenceInterval.None;

            ScheduledActionService.Add(reminder);
        }

        public static void RemoveReminder(string name)
        {
            var reminder = ScheduledActionService.Find(name);

            if (reminder != null)
                ScheduledActionService.Remove(name);
        }

        public static bool HasReminder(string name)
        {
            return ScheduledActionService.Find(name) != null;
        }
    }
}