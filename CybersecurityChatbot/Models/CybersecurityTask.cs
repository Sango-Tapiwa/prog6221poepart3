using System;

namespace CybersecurityChatbot.Models
{
    public class CybersecurityTask
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReminderDate { get; set; }

        public CybersecurityTask()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
            IsCompleted = false;
        }

        public CybersecurityTask(string title, string description, int? reminderDays = null) : this()
        {
            Title = title;
            Description = description;

            if (reminderDays.HasValue)
            {
                ReminderDate = DateTime.Now.AddDays(reminderDays.Value);
            }
        }
    }
}