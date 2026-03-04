// @GeneratedCode
using LearningManager.Logic.DataContext;
using LearningManager.Logic.Entities.App;
using LearningManager.Logic.Models;
using Microsoft.EntityFrameworkCore;
using AppTaskStatus = LearningManager.Logic.Entities.App.TaskStatus;

namespace LearningManager.Logic.Services;

/// <summary>
/// Calculates a learning schedule for a given date range.
/// 
/// Algorithm:
///  - Slots repeat every week (DayOfWeek 0=Mon … 6=Sun).
///  - Tasks are sorted deterministically by DueDate, then Status, then Id.
///  - A task occupies consecutive slot time until its EstimatedHours are consumed.
///  - If a slot has remaining time after finishing a task, the next task can use the rest.
///  - No slot is assigned a task whose DueDate has already passed.
///  - Done tasks are skipped entirely.
/// </summary>
public class SchedulingService
{
    private readonly ProjectDbContext _context;

    public SchedulingService(ProjectDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns schedule entries for every day in [<paramref name="from"/>, <paramref name="to"/>].
    /// </summary>
    public async Task<IReadOnlyList<ScheduleEntry>> GetScheduleAsync(DateTime from, DateTime to)
    {
        from = from.Date;
        to   = to.Date;

        if (from > to) return [];

        // ── 1. Load data ─────────────────────────────────────────────────
        var rawSlots = await _context.LearningSlots.AsNoTracking().ToListAsync();
        var slots = rawSlots
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ToList();

        var tasks = await _context.TaskItems
            .AsNoTracking()
            .Where(t => t.Status != AppTaskStatus.Done)
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Status == AppTaskStatus.InProgress ? 0 : 1)
            .ThenBy(t => t.Id)
            .ToListAsync();

        if (slots.Count == 0 || tasks.Count == 0)
            return [];

        // ── 2. Build the ordered list of (date, slot) pairs in the range ─
        var today = DateTime.Today;
        var slotOccurrences = new List<(DateTime Date, LearningSlot Slot)>();

        for (var day = from; day <= to; day = day.AddDays(1))
        {
            // Do not schedule tasks on past days
            if (day < today) continue;

            // Convert System DayOfWeek (Sun=0) → our convention (Mon=0)
            int ourDow = ((int)day.DayOfWeek + 6) % 7;

            foreach (var slot in slots.Where(s => s.DayOfWeek == ourDow))
            {
                slotOccurrences.Add((day, slot));
            }
        }

        // Sort by date, then start time
        slotOccurrences = slotOccurrences
            .OrderBy(x => x.Date)
            .ThenBy(x => x.Slot.StartTime)
            .ToList();

        // ── 3. Distribute tasks across slots ─────────────────────────────
        // Queue: (task, remaining hours)
        var taskQueue = new Queue<(TaskItem Task, double Remaining)>(
            tasks.Select(t => (t, t.EstimatedHours)));

        var result = new List<ScheduleEntry>();

        foreach (var (date, slot) in slotOccurrences)
        {
            var slotHours = (slot.EndTime - slot.StartTime).TotalHours;
            if (slotHours <= 0) continue;

            var remainingSlotHours = slotHours;
            var currentStart = slot.StartTime;

            while (remainingSlotHours > 0)
            {
                // Skip tasks whose due date is already past for this day
                while (taskQueue.Count > 0 && taskQueue.Peek().Task.DueDate.Date < date)
                {
                    taskQueue.Dequeue();
                }

                if (taskQueue.Count == 0)
                    break;

                var (currentTask, taskRemainingHours) = taskQueue.Dequeue();
                var assignedHours = Math.Min(taskRemainingHours, remainingSlotHours);
                if (assignedHours <= 0)
                    break;

                var assignedDuration = TimeSpan.FromTicks((long)(TimeSpan.TicksPerHour * assignedHours));
                var currentEnd = currentStart + assignedDuration;

                result.Add(new ScheduleEntry
                {
                    Date       = date,
                    StartTime  = currentStart,
                    EndTime    = currentEnd,
                    TaskTitle  = currentTask.Title,
                    TaskItemId = currentTask.Id
                });

                remainingSlotHours -= assignedHours;
                currentStart = currentEnd;

                var remainingTaskHours = taskRemainingHours - assignedHours;
                if (remainingTaskHours > 0)
                {
                    // Re-queue at front: this task still needs more time
                    var rest = taskQueue.ToList();
                    taskQueue.Clear();
                    taskQueue.Enqueue((currentTask, remainingTaskHours));
                    foreach (var item in rest)
                    {
                        taskQueue.Enqueue(item);
                    }
                }
            }

            if (taskQueue.Count == 0)
                break;
        }

        return result;
    }

    // Convenience overload: single week starting at weekStart
    public Task<IReadOnlyList<ScheduleEntry>> GetScheduleAsync(DateTime weekStart)
        => GetScheduleAsync(weekStart, weekStart.AddDays(6));
}

