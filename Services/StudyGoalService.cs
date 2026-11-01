using System.Text.Json;
using itdontholdnineitholdfifty.Models;

namespace itdontholdnineitholdfifty.Services;

public sealed class StudyGoalService : IStudyGoalService
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public StudyGoalService()
    {
        var appDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WordSteps");
        Directory.CreateDirectory(appDataDirectory);
        _filePath = Path.Combine(appDataDirectory, "study-goal.json");
    }

    public async Task<StudyGoalProgress> GetProgressAsync()
    {
        await _gate.WaitAsync();
        try
        {
            var state = await LoadCurrentStateAsync();
            return new StudyGoalProgress(state.DailyTarget, state.CompletedToday);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SetDailyTargetAsync(int dailyTarget)
    {
        if (dailyTarget is < 1 or > 200)
            throw new ArgumentOutOfRangeException(nameof(dailyTarget), "The daily target must be between 1 and 200.");

        await _gate.WaitAsync();
        try
        {
            var state = await LoadCurrentStateAsync();
            state.DailyTarget = dailyTarget;
            await SaveStateAsync(state);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task RecordQuestionAsync()
    {
        await _gate.WaitAsync();
        try
        {
            var state = await LoadCurrentStateAsync();
            state.CompletedToday++;
            await SaveStateAsync(state);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<GoalState> LoadCurrentStateAsync()
    {
        GoalState state;
        if (File.Exists(_filePath))
        {
            var json = await File.ReadAllTextAsync(_filePath);
            state = JsonSerializer.Deserialize<GoalState>(json) ?? new GoalState();
        }
        else
        {
            state = new GoalState();
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (state.Date != today)
        {
            state.Date = today;
            state.CompletedToday = 0;
            await SaveStateAsync(state);
        }
        else if (!File.Exists(_filePath))
        {
            await SaveStateAsync(state);
        }

        return state;
    }

    private async Task SaveStateAsync(GoalState state)
    {
        var temporaryPath = $"{_filePath}.tmp";
        await File.WriteAllTextAsync(temporaryPath, JsonSerializer.Serialize(state, JsonOptions));
        File.Move(temporaryPath, _filePath, true);
    }

    private sealed class GoalState
    {
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public int DailyTarget { get; set; } = 15;
        public int CompletedToday { get; set; }
    }
}
