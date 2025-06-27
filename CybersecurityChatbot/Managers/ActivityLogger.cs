using CybersecurityChatbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    public class ActivityLogger
    {
        private List<ActivityLogEntry> activities;
        private const int MaxLogEntries = 50; // Keep last 50 entries

        public ActivityLogger()
        {
            activities = new List<ActivityLogEntry>();
        }

        public void LogActivity(string action, string details)
        {
            var entry = new ActivityLogEntry(action, details);
            activities.Insert(0, entry); // Add to beginning for chronological order

            // Keep only the most recent entries
            if (activities.Count > MaxLogEntries)
            {
                activities = activities.Take(MaxLogEntries).ToList();
            }
        }

        public List<ActivityLogEntry> GetRecentActivities(int count = 10)
        {
            return activities.Take(count).ToList();
        }

        public List<ActivityLogEntry> GetAllActivities()
        {
            return activities.ToList();
        }

        public void ClearLog()
        {
            activities.Clear();
        }

        public int GetActivityCount()
        {
            return activities.Count;
        }

        public List<ActivityLogEntry> GetActivitiesByAction(string action)
        {
            return activities.Where(a => a.Action.Equals(action, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<ActivityLogEntry> GetActivitiesInDateRange(DateTime startDate, DateTime endDate)
        {
            return activities.Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate).ToList();
        }
    }
}