namespace itdontholdnineitholdfifty.Models;

public class WordQuizStat
{
    public int VocabularyWordId { get; set; }
    public int Attempts { get; set; }
    public int Correct { get; set; }
    public DateTime? LastAttemptUtc { get; set; }
}
