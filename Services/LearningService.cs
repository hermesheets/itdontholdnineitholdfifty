using itdontholdnineitholdfifty.Data;
using itdontholdnineitholdfifty.Models;

namespace itdontholdnineitholdfifty.Services;

public sealed class LearningService(
    IVocabularyRepository repository,
    IStudyGoalService studyGoalService) : ILearningService
{
    public Task<QuizProgress> GetProgressAsync() => repository.GetQuizProgressAsync();

    public async Task<IReadOnlyList<WordQuizStat>> GetWordStatsAsync() =>
        await repository.GetWordQuizStatsAsync();

    public async Task<IReadOnlyList<VocabularyWord>> GetReviewWordsAsync(int count = 20)
    {
        if (count <= 0)
            return [];

        var words = await repository.GetWordsAsync();
        var stats = await repository.GetWordQuizStatsAsync();
        var statsByWordId = stats.ToDictionary(stat => stat.VocabularyWordId);

        return words
            .OrderBy(word => statsByWordId.TryGetValue(word.Id, out var stat) && stat.Attempts > 0 ? 1 : 0)
            .ThenBy(word => statsByWordId.TryGetValue(word.Id, out var stat) && stat.Attempts > 0
                ? (double)stat.Correct / stat.Attempts
                : 0)
            .ThenBy(word => statsByWordId.TryGetValue(word.Id, out var stat)
                ? stat.LastAttemptUtc ?? DateTime.MinValue
                : DateTime.MinValue)
            .Take(count)
            .ToList();
    }

    public async Task RecordAnswerAsync(int wordId, bool isCorrect)
    {
        await repository.RecordQuizAnswerAsync(wordId, isCorrect);
        await studyGoalService.RecordQuestionAsync();
    }
}
