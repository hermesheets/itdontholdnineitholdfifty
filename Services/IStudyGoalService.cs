using itdontholdnineitholdfifty.Models;

namespace itdontholdnineitholdfifty.Services;

public interface IStudyGoalService
{
    Task<StudyGoalProgress> GetProgressAsync();
    Task SetDailyTargetAsync(int dailyTarget);
    Task RecordQuestionAsync();
}
