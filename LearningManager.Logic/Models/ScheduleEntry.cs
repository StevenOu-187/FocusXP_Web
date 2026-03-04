// @GeneratedCode
using LearningManager.Common.Contracts;

namespace LearningManager.Logic.Models;

/// <summary>
/// Represents a calculated schedule entry for a single learning slot.
/// </summary>
public class ScheduleEntry : IScheduleEntry
{
    public DateTime Date { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public string TaskTitle { get; init; } = string.Empty;
    public int TaskItemId { get; init; }
}
