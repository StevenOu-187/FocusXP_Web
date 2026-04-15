using LearningManager.Logic.Entities.App;
using LearningManager.Logic.Exceptions;
using LearningManager.Logic.Services;
using LearningManager.Test.Helpers;

namespace LearningManager.Test.LearningSlotService;

[TestClass]
public class LearningSlotServiceTests
{
    [TestMethod]
    public async Task CreateAsync_StartTimeAfterEndTime_ThrowsValidationException()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.LearningSlotService(ctx);

        var slot = new LearningSlot
        {
            DayOfWeek = 0,
            StartTime = TimeSpan.FromHours(11),
            EndTime   = TimeSpan.FromHours(9)   // Start > End
        };

        await Assert.ThrowsExactlyAsync<LearningSlotValidationException>(
            () => service.CreateAsync(slot));
    }

    [TestMethod]
    public async Task CreateAsync_StartTimeEqualsEndTime_ThrowsValidationException()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.LearningSlotService(ctx);

        var slot = new LearningSlot
        {
            DayOfWeek = 1,
            StartTime = TimeSpan.FromHours(10),
            EndTime   = TimeSpan.FromHours(10)  // Start == End
        };

        await Assert.ThrowsExactlyAsync<LearningSlotValidationException>(
            () => service.CreateAsync(slot));
    }

    [TestMethod]
    public async Task CreateAsync_OverlappingSlot_SameDay_ThrowsValidationException()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.LearningSlotService(ctx);

        // Erster Slot: 08:00–10:00
        await service.CreateAsync(new LearningSlot
        {
            DayOfWeek = 2,
            StartTime = TimeSpan.FromHours(8),
            EndTime   = TimeSpan.FromHours(10)
        });

        // Zweiter Slot überlappt: 09:00–11:00
        await Assert.ThrowsExactlyAsync<LearningSlotValidationException>(
            () => service.CreateAsync(new LearningSlot
            {
                DayOfWeek = 2,
                StartTime = TimeSpan.FromHours(9),
                EndTime   = TimeSpan.FromHours(11)
            }));
    }

    [TestMethod]
    public async Task CreateAsync_NonOverlappingSlot_SameDay_Succeeds()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.LearningSlotService(ctx);

        // Erster Slot: 08:00–10:00
        await service.CreateAsync(new LearningSlot
        {
            DayOfWeek = 3,
            StartTime = TimeSpan.FromHours(8),
            EndTime   = TimeSpan.FromHours(10)
        });

        // Zweiter Slot direkt danach: 10:00–12:00 → kein Overlap
        var created = await service.CreateAsync(new LearningSlot
        {
            DayOfWeek = 3,
            StartTime = TimeSpan.FromHours(10),
            EndTime   = TimeSpan.FromHours(12)
        });

        Assert.IsNotNull(created);
        Assert.AreEqual(TimeSpan.FromHours(10), created.StartTime);
    }

    [TestMethod]
    public async Task UpdateAsync_NonExistentId_ReturnsNull()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.LearningSlotService(ctx);

        var result = await service.UpdateAsync(999, new LearningSlot
        {
            DayOfWeek = 0,
            StartTime = TimeSpan.FromHours(8),
            EndTime   = TimeSpan.FromHours(10)
        });

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task UpdateAsync_ExistingSlot_DoesNotConflictWithItself()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.LearningSlotService(ctx);

        var created = await service.CreateAsync(new LearningSlot
        {
            DayOfWeek = 4,
            StartTime = TimeSpan.FromHours(8),
            EndTime   = TimeSpan.FromHours(10)
        });

        // Update auf denselben Zeitraum → kein false-positive Overlap
        var updated = await service.UpdateAsync(created.Id, new LearningSlot
        {
            DayOfWeek = 4,
            StartTime = TimeSpan.FromHours(8),
            EndTime   = TimeSpan.FromHours(11)  // nur EndTime verlängert
        });

        Assert.IsNotNull(updated);
        Assert.AreEqual(TimeSpan.FromHours(11), updated.EndTime);
    }

    [TestMethod]
    public async Task DeleteAsync_NonExistentId_ReturnsFalse()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.LearningSlotService(ctx);

        var result = await service.DeleteAsync(999);

        Assert.IsFalse(result);
    }
}
