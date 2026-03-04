// @GeneratedCode
namespace LearningManager.Logic.Entities.App;

/// <summary>
/// Represents a learning task / assignment.
/// </summary>
public partial class TaskItem : EntityObject
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public double EstimatedHours { get; set; }

    public TaskStatus Status { get; set; }  // Open, InProgress, Done
}
