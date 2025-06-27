using CybersecurityChatbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    public class QuizManager
    {
        private List<QuizQuestion> questions;
        private ActivityLogger activityLogger;
        private int currentQuestionIndex;
        private int score;
        private bool quizActive;

        public int CurrentQuestionIndex => currentQuestionIndex;
        public int TotalQuestions => questions.Count;

        public QuizManager(ActivityLogger logger)
        {
            activityLogger = logger;
            InitializeQuestions();
            ResetQuiz();
        }

        private void InitializeQuestions()
        {
            questions = new List<QuizQuestion>
            {
                new QuizQuestion(1, "What should you do if you receive an email asking for your password?",
                    new[] { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    2, "Reporting phishing emails helps prevent scams and protects others.", QuestionType.Multiple),

                new QuizQuestion(2, "A strong password should contain at least 8 characters with a mix of letters, numbers, and symbols.",
                    new[] { "True", "False" },
                    0, "Strong passwords are your first line of defense against unauthorized access.", QuestionType.Boolean),

                new QuizQuestion(3, "Which of the following is a sign of a phishing website?",
                    new[] { "HTTPS in the URL", "Spelling errors and poor grammar", "Professional design", "Contact information provided" },
                    1, "Phishing sites often have poor grammar and spelling errors as they're quickly created.", QuestionType.Multiple),

                new QuizQuestion(4, "Two-factor authentication (2FA) significantly improves account security.",
                    new[] { "True", "False" },
                    0, "2FA adds an extra layer of security beyond just passwords.", QuestionType.Boolean),

                new QuizQuestion(5, "What should you do when using public Wi-Fi?",
                    new[] { "Access banking websites freely", "Avoid sensitive activities", "Share your connection", "Download suspicious files" },
                    1, "Public Wi-Fi networks are often unsecured and can expose your data to hackers.", QuestionType.Multiple),

                new QuizQuestion(6, "It's safe to click on links in emails from unknown senders.",
                    new[] { "True", "False" },
                    1, "Unknown links can lead to malicious websites or download malware.", QuestionType.Boolean),

                new QuizQuestion(7, "What is social engineering in cybersecurity?",
                    new[] { "Building social networks", "Manipulating people to reveal information", "Creating social media apps", "Engineering social solutions" },
                    1, "Social engineering exploits human psychology rather than technical vulnerabilities.", QuestionType.Multiple),

                new QuizQuestion(8, "Regular software updates help protect against security vulnerabilities.",
                    new[] { "True", "False" },
                    0, "Updates often include security patches that fix known vulnerabilities.", QuestionType.Boolean),

                new QuizQuestion(9, "Which practice helps protect your privacy online?",
                    new[] { "Sharing personal info freely", "Using privacy settings", "Accepting all cookies", "Using weak passwords" },
                    1, "Privacy settings give you control over what information you share.", QuestionType.Multiple),

                new QuizQuestion(10, "Antivirus software alone is sufficient to protect against all cyber threats.",
                    new[] { "True", "False" },
                    1, "Cybersecurity requires a multi-layered approach including user awareness.", QuestionType.Boolean)
            };
        }

        public void StartQuiz()
        {
            ResetQuiz();
            quizActive = true;
        }

        public void ResetQuiz()
        {
            currentQuestionIndex = 0;
            score = 0;
            quizActive = false;
        }

        public QuizQuestion GetCurrentQuestion()
        {
            if (currentQuestionIndex >= 0 && currentQuestionIndex < questions.Count)
            {
                return questions[currentQuestionIndex];
            }
            return null;
        }

        public bool SubmitAnswer(int selectedAnswer)
        {
            if (!quizActive || currentQuestionIndex >= questions.Count) return false;

            var currentQuestion = questions[currentQuestionIndex];
            bool isCorrect = selectedAnswer == currentQuestion.CorrectAnswer;

            if (isCorrect)
            {
                score++;
            }

            return isCorrect;
        }

        public bool HasNextQuestion()
        {
            return currentQuestionIndex < questions.Count - 1;
        }

        public void NextQuestion()
        {
            if (HasNextQuestion())
            {
                currentQuestionIndex++;
            }
        }

        public int GetScore()
        {
            return score;
        }

        public double GetScorePercentage()
        {
            return (double)score / questions.Count * 100;
        }

        public bool IsQuizComplete()
        {
            return currentQuestionIndex >= questions.Count - 1;
        }
    }
}