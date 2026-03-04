// @GeneratedCode
using System.ComponentModel.DataAnnotations;

namespace LearningManager.WebApi.Models;

/// <summary>Write model for creating/updating a LearningSlot.</summary>
public class LearningSlotEditModel
{
    [Range(0, 6)]
    public int DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }
}
