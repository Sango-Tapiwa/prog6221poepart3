using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CybersecurityChatbot
{
    public class SentimentAnalyzer
    {
        private Dictionary<string, List<string>> sentimentKeywords = new Dictionary<string, List<string>>
        {
            { "worried", new List<string> {
                "worried", "concerned", "anxious", "afraid", "scared", "nervous", "fear",
                "frightened", "uneasy", "troubled", "panicked", "alarmed", "stressed",
                "concern", "worry", "scary", "dangerous", "threat", "vulnerable", "helpless"
            }},

            { "confused", new List<string> {
                "confused", "unsure", "uncertain", "puzzled", "lost", "perplexed",
                "baffled", "unclear", "don't understand", "complicated", "complex",
                "difficult", "hard to follow", "what does", "how does", "can't figure out",
                "confusing", "not sure", "explain", "clarify", "help me understand"
            }},

            { "frustrated", new List<string> {
                "frustrated", "annoyed", "irritated", "angry", "upset", "mad",
                "difficult", "hard", "impossible", "can't", "problem", "issue", "trouble",
                "not working", "doesn't work", "failed", "failing", "fed up", "tired of",
                "sick of", "hate", "annoying", "irritating", "struggle", "challenging"
            }}
        };

        private string[] positiveWords = {
            "good", "great", "excellent", "amazing", "wonderful", "fantastic", "perfect",
            "love", "like", "happy", "pleased", "satisfied", "awesome", "brilliant", "outstanding"
        };

        private string[] negativeWords = {
            "bad", "terrible", "awful", "horrible", "hate", "dislike", "angry",
            "frustrated", "disappointed", "worried", "scared", "confused", "upset",
            "annoyed", "concerned"
        };

        public string AnalyzeSentiment(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "neutral";

            string lowerText = text.ToLower();

            // Check for specific sentiment categories first
            foreach (var sentiment in sentimentKeywords)
            {
                foreach (var keyword in sentiment.Value)
                {
                    if (lowerText.Contains(keyword))
                    {
                        return sentiment.Key;
                    }
                }
            }

            // Fall back to basic positive/negative analysis
            string[] words = lowerText.Split(new char[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            int positiveCount = words.Count(word => positiveWords.Contains(word));
            int negativeCount = words.Count(word => negativeWords.Contains(word));

            if (positiveCount > negativeCount)
                return "positive";
            else if (negativeCount > positiveCount)
                return "negative";
            else
                return "neutral";
        }

        public double GetSentimentScore(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0.0;

            string[] words = text.ToLower().Split(new char[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            int positiveCount = words.Count(word => positiveWords.Contains(word));
            int negativeCount = words.Count(word => negativeWords.Contains(word));
            int totalWords = words.Length;

            if (totalWords == 0) return 0.0;

            // Return a score between -1 (very negative) and 1 (very positive)
            return (double)(positiveCount - negativeCount) / totalWords;
        }

        public bool IsPositive(string text)
        {
            return AnalyzeSentiment(text) == "positive";
        }

        public bool IsNegative(string text)
        {
            string sentiment = AnalyzeSentiment(text);
            return new[] { "negative", "worried", "confused", "frustrated" }.Contains(sentiment);
        }
    }
}