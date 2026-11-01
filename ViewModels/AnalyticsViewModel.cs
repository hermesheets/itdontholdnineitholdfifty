using System.Collections.ObjectModel;
using itdontholdnineitholdfifty.Data;
using itdontholdnineitholdfifty.Services;

namespace itdontholdnineitholdfifty.ViewModels;

public sealed class AnalyticsViewModel(
    IVocabularyRepository repository,
    ILearningService learningService) : ViewModelBase
{
    private int _questionsAnswered;
    private int _correctAnswers;
    private int _wordCoverageValue;
    private string _accuracyText = "No quiz answers yet";
    private string _wordsPracticedText = "0 words practiced";
    private string _hardestWordText = "Start a quiz to see tricky words";

    public int QuestionsAnswered { get => _questionsAnswered; private set => SetProperty(ref _questionsAnswered, value); }
    public int CorrectAnswers { get => _correctAnswers; private set => SetProperty(ref _correctAnswers, value); }
    public int WordCoverageValue { get => _wordCoverageValue; private set => SetProperty(ref _wordCoverageValue, value); }
    public string AccuracyText { get => _accuracyText; private set => SetProperty(ref _accuracyText, value); }
    public string WordsPracticedText { get => _wordsPracticedText; private set => SetProperty(ref _wordsPracticedText, value); }
    public string HardestWordText { get => _hardestWordText; private set => SetProperty(ref _hardestWordText, value); }
    public ObservableCollection<WordProgressItem> WordProgressItems { get; } = [];

    public async Task RefreshAsync()
    {
        var words = await repository.GetWordsAsync();
        var progress = await learningService.GetProgressAsync();
        var stats = await learningService.GetWordStatsAsync();
        var statsByWordId = stats.ToDictionary(stat => stat.VocabularyWordId);
        var items = words.Select(word =>
        {
            statsByWordId.TryGetValue(word.Id, out var stat);
            return new WordProgressItem(word.Word, word.Translation, stat?.Attempts ?? 0, stat?.Correct ?? 0);
        }).ToList();

        WordProgressItems.Clear();
        foreach (var item in items
                     .OrderBy(item => item.Attempts == 0 ? 1 : 0)
                     .ThenBy(item => item.Attempts == 0 ? 101 : item.AccuracyValue)
                     .ThenBy(item => item.Word))
            WordProgressItems.Add(item);

        QuestionsAnswered = progress.QuestionsAnswered;
        CorrectAnswers = progress.CorrectAnswers;
        AccuracyText = progress.QuestionsAnswered == 0
            ? "No quiz answers yet"
            : $"{(double)progress.CorrectAnswers / progress.QuestionsAnswered:P0} overall accuracy";
        var practiced = items.Where(item => item.Attempts > 0).ToList();
        WordsPracticedText = $"{practiced.Count} of {words.Count} words practiced";
        WordCoverageValue = words.Count == 0 ? 0 : (int)Math.Round((double)practiced.Count / words.Count * 100);
        var hardest = practiced.OrderBy(item => item.AccuracyValue).ThenByDescending(item => item.Attempts).FirstOrDefault();
        HardestWordText = hardest is null
            ? "Start a quiz to see tricky words"
            : $"Practice next: {hardest.Word} ({hardest.AccuracyText})";
    }
}
