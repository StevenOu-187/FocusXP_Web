using LearningManager.Logic.Entities.App;
using LearningManager.Logic.Services;
using LearningManager.Test.Helpers;

namespace LearningManager.Test.BlockedDayService;

[TestClass]
public class BlockedDayServiceTests
{
    [TestMethod]
    public async Task CreateAsync_NewDate_SavesEntry()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.BlockedDayService(ctx);

        var date    = new DateOnly(2025, 12, 24);
        var created = await service.CreateAsync(new BlockedDay { Date = date, Reason = "Heiligabend" });

        Assert.IsNotNull(created);
        Assert.AreEqual(date,          created.Date);
        Assert.AreEqual("Heiligabend", created.Reason);
    }

    [TestMethod]
    public async Task CreateAsync_ExistingDate_UpdatesReasonInsteadOfDuplicate()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.BlockedDayService(ctx);

        var date = new DateOnly(2025, 1, 1);
        await service.CreateAsync(new BlockedDay { Date = date, Reason = "Erster Grund" });

        // Gleiche Date nochmal → Reason soll aktualisiert werden
        var result = await service.CreateAsync(new BlockedDay { Date = date, Reason = "Neujahr" });

        Assert.AreEqual("Neujahr", result.Reason);

        // Sicherstellen, dass kein Duplikat entstanden ist
        var all = (await service.GetAllAsync()).ToList();
        Assert.AreEqual(1, all.Count(b => b.Date == date));
    }

    [TestMethod]
    public async Task GetBlockedDatesAsync_ReturnsOnlyDatesInRange()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.BlockedDayService(ctx);

        await service.CreateAsync(new BlockedDay { Date = new DateOnly(2025, 3, 1) });
        await service.CreateAsync(new BlockedDay { Date = new DateOnly(2025, 3, 15) }); // im Bereich
        await service.CreateAsync(new BlockedDay { Date = new DateOnly(2025, 3, 31) });

        var result = (await service.GetBlockedDatesAsync(
            new DateOnly(2025, 3, 10),
            new DateOnly(2025, 3, 20))).ToList();

        Assert.HasCount(1, result);
        Assert.AreEqual(new DateOnly(2025, 3, 15), result[0]);
    }

    [TestMethod]
    public async Task DeleteByDateAsync_NonExistentDate_ReturnsFalse()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.BlockedDayService(ctx);

        var result = await service.DeleteByDateAsync(new DateOnly(2099, 1, 1));

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task DeleteByDateAsync_ExistingDate_ReturnsTrueAndRemovesEntry()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.BlockedDayService(ctx);

        var date = new DateOnly(2025, 7, 4);
        await service.CreateAsync(new BlockedDay { Date = date, Reason = "Urlaub" });

        var deleted = await service.DeleteByDateAsync(date);
        var found   = await service.GetByDateAsync(date);

        Assert.IsTrue(deleted);
        Assert.IsNull(found);
    }

    [TestMethod]
    public async Task DeleteAsync_NonExistentId_ReturnsFalse()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.BlockedDayService(ctx);

        var result = await service.DeleteAsync(999);

        Assert.IsFalse(result);
    }
}
