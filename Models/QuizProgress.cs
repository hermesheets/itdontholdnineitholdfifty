namespace itdontholdnineitholdfifty.Models;

public class QuizProgress
{
    public int Id { get; set; } = 1;
    public int QuestionsAnswered { get; set; }
    public int CorrectAnswers { get; set; }
    public DateTime? LastPracticedUtc { get; set; }
}
