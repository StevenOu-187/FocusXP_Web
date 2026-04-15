namespace LearningManager.WebApi.Models;

public class BlockedDayEditModel
{
    public DateOnly Date { get; set; }
    public string? Reason { get; set; }
}
