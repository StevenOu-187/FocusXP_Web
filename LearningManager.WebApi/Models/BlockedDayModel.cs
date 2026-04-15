namespace LearningManager.WebApi.Models;

public class BlockedDayModel
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string? Reason { get; set; }
}
