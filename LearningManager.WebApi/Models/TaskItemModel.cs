// @GeneratedCode
namespace LearningManager.WebApi.Models;

/// <summary>Read model for TaskItem.</summary>
public class TaskItemModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public double EstimatedHours { get; set; }
    public string Status { get; set; } = string.Empty;
}
