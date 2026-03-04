// @GeneratedCode
namespace LearningManager.Common.Contracts;

/// <summary>
/// Readonly contract for a calculated schedule entry.
/// </summary>
public interface IScheduleEntry
{
    DateTime Date { get; }
    TimeSpan StartTime { get; }
    TimeSpan EndTime { get; }
    string TaskTitle { get; }
    int TaskItemId { get; }
}
