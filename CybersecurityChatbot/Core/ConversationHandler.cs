using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityChatbot
{
    // Handles the flow of the conversation with the user
    public class ConversationHandler
    {
        // Stores memory of previous topics discussed with the user
        private UserMemory memory;

        // Manages the responses returned to the user based on topics
        private ResponseManager responseManager;

        // Tracks the most recent topic discussed
        private string lastTopic = string.Empty;

        // Dictionary mapping keywords to general cybersecurity topics
        private Dictionary<string, string> keywordTopics = new Dictionary<string, string>
        {
            { "password", "password" },
            { "passwords", "password" },
            { "phishing", "phishing" },
            { "email scam", "phishing" },
            { "scam", "scam" },
            { "scams", "scam" },
            { "fraud", "scam" },
            { "browsing", "browsing" },
            { "browse", "browsing" },
            { "surf", "browsing" },
            { "internet", "browsing" },
            { "privacy", "privacy" },
            { "private", "privacy" },
            { "secure", "security" },
            { "security", "security" },
            { "protect", "security" },
            { "safe", "security" },
            { "malware", "security" },
            { "virus", "security" },
            { "hack", "security" },
            { "hacker", "security" }
        };

        // Constructor to initialize memory and response manager
        public ConversationHandler(UserMemory memorySystem, ResponseManager respManager)
        {
            memory = memorySystem;
            responseManager = respManager;
        }

        // Main method to process user input and return appropriate chatbot response
        public string HandleConversation(string input, string sentiment)
        {
            // Store in conversation history
            memory.AddToHistory($"User: {input}");

            // Return response for empty input
            if (string.IsNullOrWhiteSpace(input))
            {
                return responseManager.GetEmptyInputResponse();
            }

            // Convert input to lowercase for easier comparison
            string lowerInput = input.ToLower();

            // Handle exit commands
            if (lowerInput == "exit" || lowerInput == "quit")
            {
                return "Goodbye! Stay safe online.";
            }

            // Provide help instructions
            if (lowerInput.Contains("help") || lowerInput.Contains("what can i ask") || lowerInput.Contains("what can you do"))
            {
                return responseManager.GetHelpResponse();
            }

            // Check if input matches user's interests and store them
            memory.CheckForInterests(lowerInput);

            // Determine topic based on keywords
            string topic = IdentifyTopic(lowerInput);

            // Get a prefix based on sentiment (e.g., cheerful, concerned, etc.)
            string sentimentPrefix = responseManager.GetSentimentResponse(sentiment);

            // If no recognizable topic found, check if it may be a follow-up
            if (topic == "unknown")
            {
                // If user previously discussed a topic and current input sounds like a follow-up
                if (!string.IsNullOrEmpty(lastTopic) && IsFollowUpQuestion(lowerInput))
                {
                    return sentimentPrefix + memory.GetPersonalizedIntro() + responseManager.GetResponse(lastTopic);
                }
                return responseManager.GetUnknownResponse();
            }

            // Topic was recognized
            bool isFirstTime = !memory.HasTopicBeenDiscussed(topic); // Check if it's the first discussion of this topic
            lastTopic = topic; // Update last discussed topic
            memory.MarkTopicDiscussed(topic); // Mark topic as discussed

            // Add intro if topic has been discussed before
            string intro = isFirstTime ? "" : memory.GetPersonalizedIntro();

            // Store bot response in history
            string response = sentimentPrefix + intro + responseManager.GetResponse(topic);
            memory.AddToHistory($"Bot: {response}");

            // Return sentiment-aware response for the topic
            return response;
        }

        // Identifies topic based on keyword matches in the input
        private string IdentifyTopic(string input)
        {
            foreach (var keyword in keywordTopics.Keys)
            {
                if (input.Contains(keyword))
                {
                    return keywordTopics[keyword];
                }
            }

            return "unknown"; // Return "unknown" if no keyword matched
        }

        // Determines if input is likely a follow-up question
        private bool IsFollowUpQuestion(string input)
        {
            // Common indicators that user is asking for more info
            string[] followUpIndicators = {
                "more", "another", "tell me more", "go on", "continue",
                "elaborate", "explain", "additional", "other", "further",
                "else", "examples", "tips", "advice", "also", "and", "what about"
            };

            foreach (var indicator in followUpIndicators)
            {
                if (input.Contains(indicator))
                {
                    return true;
                }
            }

            return false;
        }
    }
}