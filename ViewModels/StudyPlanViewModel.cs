using CommunityToolkit.Mvvm.Input;
using itdontholdnineitholdfifty.Models;
using itdontholdnineitholdfifty.Services;

namespace itdontholdnineitholdfifty.ViewModels;

public sealed class StudyPlanViewModel : ViewModelBase
{
    private readonly IStudyGoalService _studyGoalService;
    private string _dailyTargetInput = "15";
    private string _goalStatus = "Set a daily target to build a steady habit.";
    private int _dailyTarget = 15;
    private int _completedToday;
    private int _remainingToday = 15;
    private int _completionPercent;
    private bool _isBusy;

    public string DailyTargetInput { get => _dailyTargetInput; set => SetProperty(ref _dailyTargetInput, value); }
    public string GoalStatus { get => _goalStatus; private set => SetProperty(ref _goalStatus, value); }
    public int DailyTarget { get => _dailyTarget; private set => SetProperty(ref _dailyTarget, value); }
    public int CompletedToday { get => _completedToday; private set => SetProperty(ref _completedToday, value); }
    public int RemainingToday { get => _remainingToday; private set => SetProperty(ref _remainingToday, value); }
    public int CompletionPercent { get => _completionPercent; private set => SetProperty(ref _completionPercent, value); }
    public IAsyncRelayCommand SaveGoalCommand { get; }

    public StudyPlanViewModel(IStudyGoalService studyGoalService)
    {
        _studyGoalService = studyGoalService;
        SaveGoalCommand = new AsyncRelayCommand(SaveGoalAsync, () => !IsBusy);
    }

    public async Task RefreshAsync()
    {
        var progress = await _studyGoalService.GetProgressAsync();
        ApplyProgress(progress);
    }

    private async Task SaveGoalAsync()
    {
        if (!int.TryParse(DailyTargetInput, out var target) || target is < 1 or > 200)
        {
            GoalStatus = "Choose a daily target from 1 to 200 answers.";
            return;
        }

        IsBusy = true;
        try
        {
            await _studyGoalService.SetDailyTargetAsync(target);
            ApplyProgress(await _studyGoalService.GetProgressAsync());
            GoalStatus = "Your daily study target was saved.";
        }
        catch (Exception exception)
        {
            GoalStatus = $"Could not save your target: {exception.Message}";
        }
        finally
        {
            IsBusy = false;
            SaveGoalCommand.NotifyCanExecuteChanged();
        }
    }

    private bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
                SaveGoalCommand.NotifyCanExecuteChanged();
        }
    }

    private void ApplyProgress(StudyGoalProgress progress)
    {
        DailyTarget = progress.DailyTarget;
        DailyTargetInput = progress.DailyTarget.ToString();
        CompletedToday = progress.CompletedToday;
        RemainingToday = progress.RemainingToday;
        CompletionPercent = progress.CompletionPercent;
    }
}
