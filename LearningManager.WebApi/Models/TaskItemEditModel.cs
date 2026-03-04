// @GeneratedCode
using System.ComponentModel.DataAnnotations;

namespace LearningManager.WebApi.Models;

/// <summary>Write model for creating/updating a TaskItem.</summary>
public class TaskItemEditModel
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    [Range(0.25, 1000)]
    public double EstimatedHours { get; set; }

    /// <summary>Allowed values: "Open", "InProgress", "Done"</summary>
    public string Status { get; set; } = "Open";
}
