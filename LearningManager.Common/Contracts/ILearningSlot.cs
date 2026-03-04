// @GeneratedCode
namespace LearningManager.Common.Contracts;

/// <summary>
/// Contract for a weekly learning time slot.
/// </summary>
public interface ILearningSlot
{
    int Id { get; }

    /// <summary>0 = Monday ... 6 = Sunday</summary>
    int DayOfWeek { get; set; }

    TimeSpan StartTime { get; set; }

    TimeSpan EndTime { get; set; }
}
