// @GeneratedCode
namespace LearningManager.Logic.Entities.App;

/// <summary>
/// Represents a weekly learning time slot.
/// </summary>
public partial class LearningSlot : EntityObject
{
    /// <summary>0 = Monday ... 6 = Sunday</summary>
    public int DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }
}
