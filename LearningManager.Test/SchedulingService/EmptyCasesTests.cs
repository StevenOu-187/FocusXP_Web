using LearningManager.Logic.Entities.App;
using LearningManager.Logic.Services;
using LearningManager.Test.Helpers;
using AppTaskStatus = LearningManager.Logic.Entities.App.TaskStatus;

namespace LearningManager.Test.SchedulingService;

[TestClass]
public class EmptyCasesTests
{
    // Hilfsmethode: heutigen Wochentag in unserer Konvention (Mo=0 … So=6)
    private static int TodayDow() => ((int)DateTime.Today.DayOfWeek + 6) % 7;

    [TestMethod]
    public async Task NoSlots_ReturnsEmptyList()
    {
        using var ctx = DbContextFactory.Create();
        ctx.TaskItems.Add(new TaskItem
        {
            Title          = "Task A",
            DueDate        = DateTime.Today.AddDays(7),
            EstimatedHours = 2,
            Status         = AppTaskStatus.Open
        });
        await ctx.SaveChangesAsync();

        var service = new Logic.Services.SchedulingService(ctx);
        var result  = await service.GetScheduleAsync(DateTime.Today, DateTime.Today.AddDays(6));

        Assert.IsEmpty(result);
    }

    [TestMethod]
    public async Task NoTasks_ReturnsEmptyList()
    {
        using var ctx = DbContextFactory.Create();
        ctx.LearningSlots.Add(new LearningSlot
        {
            DayOfWeek = TodayDow(),
            StartTime = TimeSpan.FromHours(9),
            EndTime   = TimeSpan.FromHours(11)
        });
        await ctx.SaveChangesAsync();

        var service = new Logic.Services.SchedulingService(ctx);
        var result  = await service.GetScheduleAsync(DateTime.Today, DateTime.Today.AddDays(6));

        Assert.IsEmpty(result);
    }

    [TestMethod]
    public async Task FromGreaterThanTo_ReturnsEmptyList()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.SchedulingService(ctx);

        var result = await service.GetScheduleAsync(
            DateTime.Today.AddDays(5),
            DateTime.Today.AddDays(1));

        Assert.IsEmpty(result);
    }

    [TestMethod]
    public async Task AllTasksDone_ReturnsEmptyList()
    {
        using var ctx = DbContextFactory.Create();
        ctx.LearningSlots.Add(new LearningSlot
        {
            DayOfWeek = TodayDow(),
            StartTime = TimeSpan.FromHours(9),
            EndTime   = TimeSpan.FromHours(11)
        });
        ctx.TaskItems.Add(new TaskItem
        {
            Title          = "Task A",
            DueDate        = DateTime.Today.AddDays(7),
            EstimatedHours = 2,
            Status         = AppTaskStatus.Done
        });
        await ctx.SaveChangesAsync();

        var service = new Logic.Services.SchedulingService(ctx);
        var result  = await service.GetScheduleAsync(DateTime.Today, DateTime.Today.AddDays(6));

        Assert.IsEmpty(result);
    }
}
