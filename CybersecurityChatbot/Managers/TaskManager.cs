using CybersecurityChatbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    public class TaskManager
    {
        private List<CybersecurityTask> tasks;
        private ActivityLogger activityLogger;

        public TaskManager(ActivityLogger logger)
        {
            tasks = new List<CybersecurityTask>();
            activityLogger = logger;
        }

        public void AddTask(string title, string description, int? reminderDays = null)
        {
            var task = new CybersecurityTask(title, description, reminderDays);
            tasks.Add(task);

            activityLogger.LogActivity("Task Created", $"Added task: \"{title}\"");

            if (reminderDays.HasValue)
            {
                activityLogger.LogActivity("Reminder Set", $"Reminder set for \"{title}\" in {reminderDays.Value} day(s)");
            }
        }

        public List<CybersecurityTask> GetAllTasks()
        {
            return tasks.OrderByDescending(t => t.CreatedAt).ToList();
        }

        public CybersecurityTask GetTaskById(string taskId)
        {
            return tasks.FirstOrDefault(t => t.Id == taskId);
        }

        public void ToggleTaskCompletion(string taskId)
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                activityLogger.LogActivity("Task Updated", $"Task \"{task.Title}\" marked as {(task.IsCompleted ? "completed" : "pending")}");
            }
        }

        public bool UpdateTask(string taskId, string newTitle, string newDescription, int? reminderDays = null)
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                string oldTitle = task.Title;
                task.Title = newTitle;
                task.Description = newDescription;

                if (reminderDays.HasValue)
                {
                    task.ReminderDate = DateTime.Now.AddDays(reminderDays.Value);
                }
                else
                {
                    task.ReminderDate = null;
                }

                activityLogger.LogActivity("Task Updated", $"Updated task from \"{oldTitle}\" to \"{newTitle}\"");

                if (reminderDays.HasValue)
                {
                    activityLogger.LogActivity("Reminder Updated", $"Reminder updated for \"{newTitle}\" in {reminderDays.Value} day(s)");
                }

                return true;
            }
            return false;
        }

        public bool DeleteTask(string taskId)
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                tasks.Remove(task);
                activityLogger.LogActivity("Task Deleted", $"Deleted task: \"{task.Title}\"");
                return true;
            }
            return false;
        }

        public int GetTaskCount()
        {
            return tasks.Count;
        }

        public int GetCompletedTaskCount()
        {
            return tasks.Count(t => t.IsCompleted);
        }

        public int GetPendingTaskCount()
        {
            return tasks.Count(t => !t.IsCompleted);
        }

        public List<CybersecurityTask> GetTasksWithReminders()
        {
            return tasks.Where(t => t.ReminderDate.HasValue && !t.IsCompleted).ToList();
        }

        public List<CybersecurityTask> GetOverdueTasks()
        {
            return tasks.Where(t => t.ReminderDate.HasValue &&
                                   t.ReminderDate.Value < DateTime.Now &&
                                   !t.IsCompleted).ToList();
        }

        public List<CybersecurityTask> GetCompletedTasks()
        {
            return tasks.Where(t => t.IsCompleted).OrderByDescending(t => t.CreatedAt).ToList();
        }

        public List<CybersecurityTask> GetPendingTasks()
        {
            return tasks.Where(t => !t.IsCompleted).OrderByDescending(t => t.CreatedAt).ToList();
        }

        public void ClearAllTasks()
        {
            int taskCount = tasks.Count;
            tasks.Clear();
            activityLogger.LogActivity("Tasks Cleared", $"Cleared all {taskCount} tasks");
        }

        public void ClearCompletedTasks()
        {
            var completedTasks = tasks.Where(t => t.IsCompleted).ToList();
            foreach (var task in completedTasks)
            {
                tasks.Remove(task);
            }
            activityLogger.LogActivity("Completed Tasks Cleared", $"Cleared {completedTasks.Count} completed tasks");
        }
    }
}