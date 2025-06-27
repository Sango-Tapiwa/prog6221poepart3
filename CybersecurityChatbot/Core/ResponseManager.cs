using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityChatbot
{
    public class ResponseManager
    {
        private Random random = new Random();

        // Dictionary to store responses for different keywords
        private Dictionary<string, List<string>> responses = new Dictionary<string, List<string>>
        {
            // Password responses
            { "password", new List<string> {
                "Create strong, unique passwords that are at least 16 characters long and include a mix of uppercase and lowercase letters, numbers, and symbols.",
                "Never reuse passwords across multiple accounts. Consider using a password manager to help you create and store strong, unique passwords.",
                "Enable two-factor authentication whenever possible for an extra layer of security beyond just your password.",
                "Regularly change your passwords, especially for critical accounts like banking and email.",
                "A good password strategy is to use a passphrase - a series of random words with numbers and symbols mixed in.",
                "Consider using a different email address for your most important accounts to add an extra layer of security."
            }},
            
            // Phishing responses
            { "phishing", new List<string> {
                "Phishing is when scammers try to trick you into giving up private info by pretending to be someone you trust. Always verify the sender before clicking links.",
                "Be suspicious of unexpected emails, especially those creating urgency or asking for personal information.",
                "Check email addresses carefully - phishers often use addresses that look similar to legitimate ones but have small differences.",
                "Hover over links before clicking to see where they actually lead. If in doubt, navigate to the website directly instead of clicking the link.",
                "Legitimate organizations will never ask for sensitive information via email. When in doubt, contact the company directly using their official website or phone number.",
                "Be wary of emails with poor spelling and grammar, as these are often indicators of phishing attempts."
            }},
            
            // Safe browsing responses
            { "browsing", new List<string> {
                "Safe browsing means avoiding dangerous websites and downloads. Stick to secure sites (those starting with https), and be mindful of pop-ups or weird downloads.",
                "Keep your browser and its extensions updated to protect against known vulnerabilities.",
                "Consider using privacy-focused browsers or extensions that block trackers and unwanted ads.",
                "Be cautious when downloading files from the internet - verify the source is legitimate before proceeding.",
                "Use a VPN when connecting to public Wi-Fi networks to encrypt your connection and protect your data.",
                "Clear your browsing history, cookies, and cache regularly to maintain privacy and improve performance."
            }},
            
            // Privacy responses
            { "privacy", new List<string> {
                "Regularly review and adjust your privacy settings on social media and other online accounts.",
                "Be mindful of what information you share online - once it's out there, it can be difficult to remove.",
                "Consider using a VPN to encrypt your internet traffic and protect your privacy, especially on public WiFi.",
                "Regularly clear your browsing history, cookies, and cached data to minimize tracking.",
                "Use private browsing modes when accessing sensitive information on shared computers.",
                "Be careful about which apps you grant permissions to - many request access to more data than they actually need."
            }},
            
            // Security responses
            { "security", new List<string> {
                "Keep all your software updated with the latest security patches to protect against known vulnerabilities.",
                "Use antivirus software and keep it updated to protect against malware and other threats.",
                "Be cautious about which apps you install and what permissions you grant them.",
                "Regularly back up your important data to protect against ransomware and other data loss scenarios.",
                "Consider using encrypted messaging apps for sensitive communications.",
                "Be wary of USB drives from unknown sources, as they can contain malware."
            }},
            
            // Scam responses
            { "scam", new List<string> {
                "If an offer seems too good to be true, it probably is. Be skeptical of amazing deals or unexpected winnings.",
                "Legitimate organizations won't ask for sensitive information via email or text. When in doubt, contact the organization directly using official contact information.",
                "Be wary of pressure tactics - scammers often create urgency to prevent you from thinking clearly.",
                "Research unfamiliar companies or products before making purchases or investments.",
                "Never send money to someone you haven't met in person, especially if they're requesting unusual payment methods like gift cards or wire transfers.",
                "Be cautious of romance scams - people who quickly profess love but always have excuses for why they can't meet in person."
            }},

            // General cybersecurity tips
            { "general", new List<string> {
                "Regularly update all your software and devices to patch security vulnerabilities.",
                "Be mindful of what you share online and adjust your privacy settings accordingly.",
                "Use unique, strong passwords for each of your accounts and consider a password manager.",
                "Always verify the source before clicking links or downloading attachments.",
                "Enable two-factor authentication wherever possible for an extra layer of security.",
                "Regularly back up important data to protect against ransomware and device failure."
            }},

            // Greeting responses
            { "greeting", new List<string> {
                "Hello! I'm here to help you stay safe online. What would you like to know about cybersecurity?",
                "Hi there! Ready to learn about cybersecurity? Ask me anything!",
                "Welcome! I'm your cybersecurity assistant. How can I help protect you online today?"
            }},

            // Help responses
            { "help", new List<string> {
                "I can provide information about password safety, phishing, safe browsing, privacy, security, and scams. What would you like to know about?",
                "You can ask me about various cybersecurity topics like password safety, phishing, privacy, and more. What are you interested in learning about?",
                "I'm here to help with cybersecurity questions! Try asking about password security, phishing protection, or safe browsing habits.",
                "Need some guidance on staying safe online? Ask me about topics like strong passwords, recognizing scams, or protecting your privacy."
            }},

            // Empty input responses
            { "empty", new List<string> {
                "I didn't quite catch that. Could you please rephrase?",
                "I'm not sure what you're asking. Could you try again?",
                "I didn't understand your question. Can you rephrase it?",
                "I need more information to help you. Could you please elaborate?"
            }},

            // Unknown input responses
            { "unknown", new List<string> {
                "I'm not familiar with that topic yet. I can tell you about password safety, phishing, safe browsing, privacy, or security. What would you like to know?",
                "I don't have information on that specific topic. Would you like to know about password safety, phishing, safe browsing, privacy, or security instead?",
                "That's outside my current knowledge. I can help with common cybersecurity topics like passwords, phishing, browsing safety, and more.",
                "I'm still learning about that area. Can I help you with something related to passwords, online scams, or general internet safety?"
            }}
        };

        // Dictionary for sentiment-based responses
        private Dictionary<string, List<string>> sentimentResponses = new Dictionary<string, List<string>>
        {
            { "worried", new List<string> {
                "I understand your concern. Cybersecurity can seem overwhelming, but taking small steps can make a big difference. ",
                "It's natural to feel worried about online threats. Let me help you understand how to protect yourself. ",
                "I hear your concern. Many people feel the same way about online security. Let's focus on practical steps you can take. ",
                "Those concerns are completely valid. The digital world can be intimidating, but with the right knowledge, you can navigate it safely. "
            }},

            { "confused", new List<string> {
                "It sounds like you're finding this information a bit complex. Let me try to explain it more clearly. ",
                "Cybersecurity can be confusing! Let me break it down into simpler steps. ",
                "I understand this might be overwhelming. Let's focus on the basics first. ",
                "That's a common point of confusion. Let me simplify it for you. "
            }},

            { "frustrated", new List<string> {
                "I can hear your frustration. Technology challenges can be difficult, but we'll work through this together. ",
                "It's frustrating when technology doesn't work as expected. Let me try to help simplify things. ",
                "I understand your frustration. Let's try to find a straightforward approach to address your concerns. ",
                "That certainly sounds frustrating. Let's tackle this one step at a time to make it more manageable. "
            }},

            { "positive", new List<string> {
                "I'm glad you're taking security seriously! ",
                "That's a great attitude towards cybersecurity! ",
                "Excellent! Your proactive approach to security is commendable. ",
                "Wonderful! It's great to see you're engaged with cybersecurity. "
            }},

            { "negative", new List<string> {
                "Don't worry, I'm here to help you stay secure. ",
                "I understand your concerns. Let me help you feel more confident about your security. ",
                "It's okay to feel uncertain about security - let's work through this together. ",
                "Your concerns are valid. Let me provide some guidance to help you feel more secure. "
            }}
        };

        // Get response for a specific keyword
        public string GetResponse(string keyword)
        {
            if (responses.ContainsKey(keyword))
            {
                int index = random.Next(responses[keyword].Count);
                return responses[keyword][index];
            }

            // If keyword not found, return a general response
            int generalIndex = random.Next(responses["general"].Count);
            return responses["general"][generalIndex];
        }

        // Get response for unknown input
        public string GetUnknownResponse()
        {
            int index = random.Next(responses["unknown"].Count);
            return responses["unknown"][index];
        }

        // Get response for empty input
        public string GetEmptyInputResponse()
        {
            int index = random.Next(responses["empty"].Count);
            return responses["empty"][index];
        }

        // Get help response
        public string GetHelpResponse()
        {
            int index = random.Next(responses["help"].Count);
            return responses["help"][index];
        }

        // Get sentiment-based response
        public string GetSentimentResponse(string sentiment)
        {
            if (string.IsNullOrWhiteSpace(sentiment))
            {
                return "";
            }

            string lowerSentiment = sentiment.ToLower();

            if (sentimentResponses.ContainsKey(lowerSentiment))
            {
                int index = random.Next(sentimentResponses[lowerSentiment].Count);
                return sentimentResponses[lowerSentiment][index];
            }

            return "";
        }

        // Get random response from array
        public string GetRandomResponse(string[] responseOptions)
        {
            if (responseOptions == null || responseOptions.Length == 0)
                return GetResponse("general");

            return responseOptions[random.Next(responseOptions.Length)];
        }
    }
}