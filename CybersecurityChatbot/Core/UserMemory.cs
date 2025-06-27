using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityChatbot
{
    // This class stores and manages the user's interaction history and interests
    public class UserMemory
    {
        // Stores the user's name
        private string userName = string.Empty;

        // List of interest descriptions the user has shown interest in
        private List<string> userInterests = new List<string>();

        // Tracks whether specific topics have been discussed or not
        private Dictionary<string, bool> topicsDiscussed = new Dictionary<string, bool>();

        // Counts how many times a keyword/topic has been mentioned
        private Dictionary<string, int> topicMentionCounts = new Dictionary<string, int>();

        // User preferences storage
        private Dictionary<string, object> userPreferences = new Dictionary<string, object>();

        // Conversation history
        private List<string> conversationHistory = new List<string>();

        // Maps keywords to more readable interest descriptions
        private Dictionary<string, string> interestKeywords = new Dictionary<string, string>
        {
            { "password", "password security" },
            { "phishing", "phishing awareness" },
            { "privacy", "privacy" },
            { "security", "security" },
            { "malware", "malware protection" },
            { "vpn", "VPNs" },
            { "encryption", "encryption" },
            { "browsing", "safe browsing" },
            { "hack", "hacking prevention" },
            { "scam", "scam prevention" },
            { "authentication", "authentication" },
            { "2fa", "two-factor authentication" },
            { "two-factor", "two-factor authentication" },
            { "virus", "virus protection" },
            { "firewall", "firewall protection" },
            { "social media", "social media privacy" },
            { "cookies", "browser cookies" },
            { "tracking", "online tracking" }
        };

        // Stores the last topic the user showed interest in
        private string lastTopic = string.Empty;

        // Set of discussed topics for quick look-up
        private HashSet<string> discussedTopics = new HashSet<string>();

        // Constructor initializes the topic tracking dictionaries
        public UserMemory()
        {
            foreach (var keyword in interestKeywords.Keys)
            {
                topicsDiscussed[keyword] = false;
                topicMentionCounts[keyword] = 0;
            }
        }

        // Stores the user's name
        public void StoreName(string name)
        {
            userName = name;
        }

        // Returns the stored user's name or a default "friend" if none exists
        public string GetName()
        {
            return string.IsNullOrEmpty(userName) ? "friend" : userName;
        }

        // Analyzes input to detect and store interests based on keywords
        public void CheckForInterests(string input)
        {
            string lowerInput = input.ToLower();

            foreach (var keyword in interestKeywords.Keys)
            {
                if (lowerInput.Contains(keyword))
                {
                    // Increment the count for this keyword/topic
                    topicMentionCounts[keyword]++;

                    // Add to user's interests if not already recorded
                    if (!userInterests.Contains(interestKeywords[keyword]))
                        userInterests.Add(interestKeywords[keyword]);

                    // Mark topic as discussed
                    topicsDiscussed[keyword] = true;

                    // Update lastTopic to the matched interest
                    lastTopic = interestKeywords[keyword];
                }
            }
        }

        // Checks if a specific interest is already known
        public bool HasInterest(string interest)
        {
            return userInterests.Contains(interest);
        }

        // Returns all interests the user has shown so far
        public List<string> GetInterests()
        {
            return new List<string>(userInterests);
        }

        // Checks if a topic has been previously discussed
        public bool HasTopicBeenDiscussed(string topic)
        {
            return discussedTopics.Contains(topic);
        }

        // Marks a topic as discussed by adding it to the set
        public void MarkTopicDiscussed(string topic)
        {
            discussedTopics.Add(topic);
        }

        // Returns a personalized introduction based on user's last interest
        public string GetPersonalizedIntro()
        {
            if (string.IsNullOrEmpty(lastTopic))
            {
                return "";
            }

            if (!string.IsNullOrEmpty(userName))
            {
                return $"Since you've shown interest in {lastTopic}, ";
            }

            return $"Since you're interested in {lastTopic}, ";
        }

        // Add message to conversation history
        public void AddToHistory(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm");
            conversationHistory.Add($"{timestamp}: {message}");

            // Keep only last 20 messages
            if (conversationHistory.Count > 20)
            {
                conversationHistory.RemoveAt(0);
            }
        }

        // Get conversation history
        public List<string> GetConversationHistory()
        {
            return new List<string>(conversationHistory);
        }
    }
}