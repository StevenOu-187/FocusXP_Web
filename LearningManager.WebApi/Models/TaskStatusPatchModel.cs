// @GeneratedCode
namespace LearningManager.WebApi.Models;

/// <summary>Payload for PATCH /api/taskitems/{id}/status</summary>
public class TaskStatusPatchModel
{
    /// <summary>Allowed values: "Open", "InProgress", "Done"</summary>
    public string Status { get; set; } = string.Empty;
}
