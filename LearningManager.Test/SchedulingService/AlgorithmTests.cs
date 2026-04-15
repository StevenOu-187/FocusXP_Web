using LearningManager.Logic.Entities.App;
using LearningManager.Test.Helpers;
using AppTaskStatus = LearningManager.Logic.Entities.App.TaskStatus;

namespace LearningManager.Test.SchedulingService;

[TestClass]
public class AlgorithmTests
{
    private static int DowFor(DateTime date) => ((int)date.DayOfWeek + 6) % 7;

    [TestMethod]
    public async Task SingleTask_FitsExactlyInSlot_ProducesOneEntry()
    {
        using var ctx = DbContextFactory.Create();
        var today = DateTime.Today;

        ctx.LearningSlots.Add(new LearningSlot
        {
            DayOfWeek = DowFor(today),
            StartTime = TimeSpan.FromHours(8),
            EndTime   = TimeSpan.FromHours(10)   // 2 h
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
        var result  = await service.GetScheduleAsync(today, today);

        Assert.HasCount(1, result);
        Assert.AreEqual(TimeSpan.FromHours(8),  result[0].StartTime);
        Assert.AreEqual(TimeSpan.FromHours(10), result[0].EndTime);
        Assert.AreEqual("Task A", result[0].TaskTitle);
    }

    [TestMethod]
    public async Task SingleTask_LargerThanOneSlot_SpansAcrossTwoSlots()
    {
        using var ctx = DbContextFactory.Create();
        var day0 = DateTime.Today;
        var day1 = day0.AddDays(1);

        // Slot am heutigen Tag: 2 h
        ctx.LearningSlots.Add(new LearningSlot
        {
            DayOfWeek = DowFor(day0),
            StartTime = TimeSpan.FromHours(8),
            EndTime   = TimeSpan.FromHours(10)
        });
        // Slot am morgigen Tag: 2 h
        ctx.LearningSlots.Add(new LearningSlot
        {
            DayOfWeek = DowFor(day1),
            StartTime = TimeSpan.FromHours(8),
            EndTime   = TimeSpan.FromHours(10)
        });
        // Task benötigt 3 h → passt nicht in einen Slot
        ctx.TaskItems.Add(new TaskItem
        {
            Title          = "Großer Task",
            DueDate        = day0.AddDays(7),
            EstimatedHours = 3,
            Status         = AppTaskStatus.Open
        });
        await ctx.SaveChangesAsync();

        var service = new Logic.Services.SchedulingService(ctx);
        var result  = await service.GetScheduleAsync(day0, day1);

        Assert.HasCount(2, result);

        var entry0 = result.Single(e => e.Date == day0);
        var entry1 = result.Single(e => e.Date == day1);

        Assert.AreEqual(2.0, (entry0.EndTime - entry0.StartTime).TotalHours, 1e-9);
        Assert.AreEqual(1.0, (entry1.EndTime - entry1.StartTime).TotalHours, 1e-9);
        Assert.AreEqual("Großer Task", entry0.TaskTitle);
        Assert.AreEqual("Großer Task", entry1.TaskTitle);
    }

    [TestMethod]
    public async Task MultipleTasksDistributedAcrossSlots()
    {
        using var ctx = DbContextFactory.Create();
        var today = DateTime.Today;

        // Ein Slot: 4 h
        ctx.LearningSlots.Add(new LearningSlot
        {
            DayOfWeek = DowFor(today),
            StartTime = TimeSpan.FromHours(8),
            EndTime   = TimeSpan.FromHours(12)
        });
        // Task 1: 2 h, Task 2: 2 h → beide passen in den einen Slot
        ctx.TaskItems.Add(new TaskItem
        {
            Title          = "Task 1",
            DueDate        = today.AddDays(3),
            EstimatedHours = 2,
            Status         = AppTaskStatus.Open
        });
        ctx.TaskItems.Add(new TaskItem
        {
            Title          = "Task 2",
            DueDate        = today.AddDays(5),
            EstimatedHours = 2,
            Status         = AppTaskStatus.Open
        });
        await ctx.SaveChangesAsync();

        var service = new Logic.Services.SchedulingService(ctx);
        var result  = await service.GetScheduleAsync(today, today);

        Assert.HasCount(2, result);
        Assert.AreEqual("Task 1", result[0].TaskTitle);
        Assert.AreEqual("Task 2", result[1].TaskTitle);
        // Zeitblöcke schließen nahtlos aneinander an
        Assert.AreEqual(result[0].EndTime, result[1].StartTime);
    }

    [TestMethod]
    public async Task SlotWithZeroDuration_IsSkipped()
    {
        using var ctx = DbContextFactory.Create();
        var today = DateTime.Today;

        // Defekter Slot: Start == End
        ctx.LearningSlots.Add(new LearningSlot
        {
            DayOfWeek = DowFor(today),
            StartTime = TimeSpan.FromHours(9),
            EndTime   = TimeSpan.FromHours(9)   // 0 h
        });
        ctx.TaskItems.Add(new TaskItem
        {
            Title          = "Task A",
            DueDate        = today.AddDays(7),
            EstimatedHours = 1,
            Status         = AppTaskStatus.Open
        });
        await ctx.SaveChangesAsync();

        var service = new Logic.Services.SchedulingService(ctx);
        var result  = await service.GetScheduleAsync(today, today);

        Assert.IsEmpty(result);
    }

    [TestMethod]
    public async Task AllSlotsConsumed_BeforeAllTasksAssigned_RemainingTasksNotInResult()
    {
        using var ctx = DbContextFactory.Create();
        var today = DateTime.Today;

        // Nur 1 h verfügbar
        ctx.LearningSlots.Add(new LearningSlot
        {
            DayOfWeek = DowFor(today),
            StartTime = TimeSpan.FromHours(9),
            EndTime   = TimeSpan.FromHours(10)   // 1 h
        });
        // Task 1: 1 h — passt komplett rein
        ctx.TaskItems.Add(new TaskItem
        {
            Title          = "Task 1",
            DueDate        = today.AddDays(2),
            EstimatedHours = 1,
            Status         = AppTaskStatus.Open
        });
        // Task 2: 2 h — kein Platz mehr
        ctx.TaskItems.Add(new TaskItem
        {
            Title          = "Task 2",
            DueDate        = today.AddDays(3),
            EstimatedHours = 2,
            Status         = AppTaskStatus.Open
        });
        await ctx.SaveChangesAsync();

        var service = new Logic.Services.SchedulingService(ctx);
        var result  = await service.GetScheduleAsync(today, today);

        Assert.HasCount(1, result);
        Assert.AreEqual("Task 1", result[0].TaskTitle);
    }
}
