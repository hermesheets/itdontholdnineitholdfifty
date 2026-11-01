namespace itdontholdnineitholdfifty.Models;

public sealed record StudyGoalProgress(int DailyTarget, int CompletedToday)
{
    public int RemainingToday => Math.Max(0, DailyTarget - CompletedToday);
    public int CompletionPercent => DailyTarget == 0
        ? 0
        : Math.Min(100, (int)Math.Round((double)CompletedToday / DailyTarget * 100));
}
