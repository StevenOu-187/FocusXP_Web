namespace LearningManager.Logic.Entities.App;

/// <summary>
/// Represents a single blocked day on which no tasks will be scheduled.
/// </summary>
public class BlockedDay : EntityObject
{
    public DateOnly Date { get; set; }

    /// <summary>Optional reason, e.g. "Feiertag", "Urlaub", "Krank".</summary>
    public string? Reason { get; set; }
}
