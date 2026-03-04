// @GeneratedCode
namespace LearningManager.WebApi.Models;

/// <summary>Readonly model returned by the schedule endpoint.</summary>
public class ScheduleEntryModel
{
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public int TaskItemId { get; set; }
}
