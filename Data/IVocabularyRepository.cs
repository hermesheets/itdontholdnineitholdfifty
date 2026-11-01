using itdontholdnineitholdfifty.Models;

namespace itdontholdnineitholdfifty.Data;

public interface IVocabularyRepository
{
    Task<List<VocabularyWord>> GetWordsAsync();
    Task AddWordAsync(VocabularyWord word);
    Task UpdateWordAsync(VocabularyWord word);
    Task DeleteWordAsync(int id);
    Task<QuizProgress> GetQuizProgressAsync();
    Task<List<WordQuizStat>> GetWordQuizStatsAsync();
    Task RecordQuizAnswerAsync(int wordId, bool isCorrect);
}
