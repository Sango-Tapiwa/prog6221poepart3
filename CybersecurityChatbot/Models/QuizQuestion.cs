namespace CybersecurityChatbot.Models
{
    public class QuizQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string[] Options { get; set; }
        public int CorrectAnswer { get; set; }
        public string Explanation { get; set; }
        public QuestionType Type { get; set; }

        public QuizQuestion(int id, string question, string[] options, int correctAnswer, string explanation, QuestionType type = QuestionType.Multiple)
        {
            Id = id;
            Question = question;
            Options = options;
            CorrectAnswer = correctAnswer;
            Explanation = explanation;
            Type = type;
        }
    }

    public enum QuestionType
    {
        Multiple,
        Boolean
    }
}