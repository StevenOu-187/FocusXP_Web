using LearningManager.Logic.Entities.App;
using LearningManager.Test.Helpers;
using AppTaskStatus = LearningManager.Logic.Entities.App.TaskStatus;

namespace LearningManager.Test.SchedulingService;

[TestClass]
public class FilteringTests
{
    private static int DowFor(DateTime date) => ((int)date.DayOfWeek + 6) % 7;

    [TestMethod]
    public async Task TaskWithPastDueDate_IsSkipped()
    {
        using var ctx = DbContextFactory.Create();
        var today = DateTime.Today;

        ctx.LearningSlots.Add(new LearningSlot
        {
            DayOfWeek = DowFor(today),
            StartTime = TimeSpan.FromHours(9),
            EndTime   = TimeSpan.FromHours(11)
        });
        // DueDate liegt in der Vergangenheit → soll nicht eingeplant werden
        ctx.TaskItems.Add(new TaskItem
        {
            Title          = "Abgelaufener Task",
            DueDate        = today.AddDays(-1),
            EstimatedHours = 2,
            Status         = AppTaskStatus.Open
        });
        await ctx.SaveChangesAsync();

        var service = new Logic.Services.SchedulingService(ctx);
        var result  = await service.GetScheduleAsync(today, today);

        Assert.IsEmpty(result);
    }

    [TestMethod]
    public async Task BlockedDay_ProducesNoEntriesForThatDay()
    {
        using var ctx = DbContextFactory.Create();
        var today = DateTime.Today;

        ctx.LearningSlots.Add(new LearningSlot
        {
            DayOfWeek = DowFor(today),
            StartTime = TimeSpan.FromHours(9),
            EndTime   = TimeSpan.FromHours(11)
        });
        ctx.TaskItems.Add(new TaskItem
        {
            Title          = "Task A",
            DueDate        = today.AddDays(7),
            EstimatedHours = 2,
            Status         = AppTaskStatus.Open
        });
        ctx.BlockedDays.Add(new BlockedDay
        {
            Date   = DateOnly.FromDateTime(today),
            Reason = "Feiertag"
        });
        await ctx.SaveChangesAsync();

        var service = new Logic.Services.SchedulingService(ctx);
        var result  = await service.GetScheduleAsync(today, today);

        Assert.IsEmpty(result);
    }

    [TestMethod]
    public async Task InProgressTask_PrioritizedOverOpenTask_WithSameDueDate()
    {
        using var ctx = DbContextFactory.Create();
        var today   = DateTime.Today;
        var dueDate = today.AddDays(5);

        // Slot: nur 1 h → nur ein Task passt rein
        ctx.LearningSlots.Add(new LearningSlot
        {
            DayOfWeek = DowFor(today),
            StartTime = TimeSpan.FromHours(9),
            EndTime   = TimeSpan.FromHours(10)
        });
        // Open-Task: gleicher DueDate, wird zuerst in DB gespeichert
        ctx.TaskItems.Add(new TaskItem
        {
            Title          = "Open Task",
            DueDate        = dueDate,
            EstimatedHours = 1,
            Status         = AppTaskStatus.Open
        });
        // InProgress-Task: gleicher DueDate, soll trotzdem vorgezogen werden
        ctx.TaskItems.Add(new TaskItem
        {
            Title          = "InProgress Task",
            DueDate        = dueDate,
            EstimatedHours = 1,
            Status         = AppTaskStatus.InProgress
        });
        await ctx.SaveChangesAsync();

        var service = new Logic.Services.SchedulingService(ctx);
        var result  = await service.GetScheduleAsync(today, today);

        Assert.HasCount(1, result);
        Assert.AreEqual("InProgress Task", result[0].TaskTitle);
    }

    [TestMethod]
    public async Task PastDay_IsNotScheduled()
    {
        using var ctx = DbContextFactory.Create();
        var yesterday = DateTime.Today.AddDays(-1);
        var today     = DateTime.Today;

        // Slot für gestern und heute
        ctx.LearningSlots.Add(new LearningSlot
        {
            DayOfWeek = DowFor(yesterday),
            StartTime = TimeSpan.FromHours(9),
            EndTime   = TimeSpan.FromHours(11)
        });
        ctx.LearningSlots.Add(new LearningSlot
        {
            DayOfWeek = DowFor(today),
            StartTime = TimeSpan.FromHours(9),
            EndTime   = TimeSpan.FromHours(11)
        });
        ctx.TaskItems.Add(new TaskItem
        {
            Title          = "Task A",
            DueDate        = today.AddDays(7),
            EstimatedHours = 2,
            Status         = AppTaskStatus.Open
        });
        await ctx.SaveChangesAsync();

        var service = new Logic.Services.SchedulingService(ctx);
        // Range schließt gestern ein — gestern darf aber nicht erscheinen
        var result  = await service.GetScheduleAsync(yesterday, today);

        Assert.IsTrue(result.All(e => e.Date >= today),
            "Vergangene Tage dürfen nicht eingeplant werden.");
    }
}
