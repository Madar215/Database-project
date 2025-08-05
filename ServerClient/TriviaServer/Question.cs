namespace TriviaServer
{
    public class Question
    {
        public string QuestionText { get; set; }
        public string Answer1Text { get; set; }
        public string Answer2Text { get; set; }
        public string Answer3Text { get; set; }
        public string Answer4Text { get; set; }
        public int CorrectAnswer { get; set; }
    }
}
