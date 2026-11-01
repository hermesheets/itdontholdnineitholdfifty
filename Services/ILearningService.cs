using itdontholdnineitholdfifty.Models;

namespace itdontholdnineitholdfifty.Services;

public interface ILearningService
{
    Task<QuizProgress> GetProgressAsync();
    Task<IReadOnlyList<WordQuizStat>> GetWordStatsAsync();
    Task<IReadOnlyList<VocabularyWord>> GetReviewWordsAsync(int count = 20);
    Task RecordAnswerAsync(int wordId, bool isCorrect);
}
