using CommunityToolkit.Mvvm.Input;
using itdontholdnineitholdfifty.Data;
using itdontholdnineitholdfifty.Services;

namespace itdontholdnineitholdfifty.ViewModels;

public sealed class DashboardViewModel(
    IVocabularyRepository repository,
    ILearningService learningService,
    Action startQuiz) : ViewModelBase
{
    private int _wordCount;
    private int _questionsAnswered;
    private int _correctAnswers;
    private int _wordCoverageValue;
    private string _scoreSummary = "0 correct answers";
    private string _accuracyText = "No quiz answers yet";
    private string _wordsPracticedText = "0 words practiced";
    private string _hardestWordText = "Start a quiz to see tricky words";

    public int WordCount { get => _wordCount; private set => SetProperty(ref _wordCount, value); }
    public int QuestionsAnswered { get => _questionsAnswered; private set => SetProperty(ref _questionsAnswered, value); }
    public int CorrectAnswers { get => _correctAnswers; private set => SetProperty(ref _correctAnswers, value); }
    public int WordCoverageValue { get => _wordCoverageValue; private set => SetProperty(ref _wordCoverageValue, value); }
    public string ScoreSummary { get => _scoreSummary; private set => SetProperty(ref _scoreSummary, value); }
    public string AccuracyText { get => _accuracyText; private set => SetProperty(ref _accuracyText, value); }
    public string WordsPracticedText { get => _wordsPracticedText; private set => SetProperty(ref _wordsPracticedText, value); }
    public string HardestWordText { get => _hardestWordText; private set => SetProperty(ref _hardestWordText, value); }
    public IRelayCommand StartQuizCommand { get; } = new RelayCommand(startQuiz);

    public async Task RefreshAsync()
    {
        var words = await repository.GetWordsAsync();
        var progress = await learningService.GetProgressAsync();
        var stats = await learningService.GetWordStatsAsync();
        var statsByWordId = stats.ToDictionary(stat => stat.VocabularyWordId);
        var practiced = words
            .Select(word => statsByWordId.TryGetValue(word.Id, out var stat) ? stat : null)
            .Where(stat => stat is { Attempts: > 0 })
            .ToList();
        var hardest = practiced
            .OrderBy(stat => (double)stat!.Correct / stat.Attempts)
            .ThenByDescending(stat => stat!.Attempts)
            .FirstOrDefault();

        WordCount = words.Count;
        QuestionsAnswered = progress.QuestionsAnswered;
        CorrectAnswers = progress.CorrectAnswers;
        ScoreSummary = $"{progress.CorrectAnswers} correct of {progress.QuestionsAnswered} answers";
        AccuracyText = progress.QuestionsAnswered == 0
            ? "No quiz answers yet"
            : $"{(double)progress.CorrectAnswers / progress.QuestionsAnswered:P0} overall accuracy";
        WordsPracticedText = $"{practiced.Count} of {words.Count} words practiced";
        WordCoverageValue = words.Count == 0 ? 0 : (int)Math.Round((double)practiced.Count / words.Count * 100);
        HardestWordText = hardest is null
            ? "Start a quiz to see tricky words"
            : $"Practice next: {words.First(word => word.Id == hardest.VocabularyWordId).Word}";
    }
}
