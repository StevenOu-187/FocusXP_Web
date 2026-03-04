// @GeneratedCode
namespace LearningManager.Common.Contracts;

/// <summary>
/// Contract for a learning task / assignment.
/// </summary>
public interface ITaskItem
{
    int Id { get; }
    string Title { get; set; }
    string Description { get; set; }
    DateTime DueDate { get; set; }
    double EstimatedHours { get; set; }
    // Status stored as string to avoid reference to Logic assembly from Common
    string Status { get; set; }
}
