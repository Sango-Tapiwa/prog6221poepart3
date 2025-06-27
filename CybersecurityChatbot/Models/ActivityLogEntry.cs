using System;

namespace CybersecurityChatbot.Models
{
    public class ActivityLogEntry
    {
        public string Id { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }

        public ActivityLogEntry()
        {
            Id = Guid.NewGuid().ToString();
            Timestamp = DateTime.Now;
        }

        public ActivityLogEntry(string action, string details) : this()
        {
            Action = action;
            Details = details;
        }
    }
}