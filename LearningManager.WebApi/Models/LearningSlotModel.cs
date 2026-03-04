// @GeneratedCode
namespace LearningManager.WebApi.Models;

/// <summary>Read model for LearningSlot.</summary>
public class LearningSlotModel
{
    public int Id { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
