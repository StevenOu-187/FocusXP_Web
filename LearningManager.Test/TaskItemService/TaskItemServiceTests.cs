using LearningManager.Logic.Entities.App;
using LearningManager.Logic.Services;
using LearningManager.Test.Helpers;
using AppTaskStatus = LearningManager.Logic.Entities.App.TaskStatus;

namespace LearningManager.Test.TaskItemService;

[TestClass]
public class TaskItemServiceTests
{
    [TestMethod]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.TaskItemService(ctx);

        var result = await service.GetByIdAsync(999);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task CreateAsync_SavesItemAndReturnsWithId()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.TaskItemService(ctx);

        var item = new TaskItem
        {
            Title          = "Neue Aufgabe",
            Description    = "Beschreibung",
            DueDate        = DateTime.Today.AddDays(7),
            EstimatedHours = 3,
            Status         = AppTaskStatus.Open
        };

        var created = await service.CreateAsync(item);

        Assert.IsGreaterThan(0, created.Id);
        Assert.AreEqual("Neue Aufgabe", created.Title);
    }

    [TestMethod]
    public async Task UpdateAsync_NonExistentId_ReturnsNull()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.TaskItemService(ctx);

        var result = await service.UpdateAsync(999, new TaskItem
        {
            Title          = "Update",
            DueDate        = DateTime.Today.AddDays(5),
            EstimatedHours = 1,
            Status         = AppTaskStatus.Open
        });

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task UpdateAsync_ExistingItem_UpdatesAllFields()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.TaskItemService(ctx);

        var created = await service.CreateAsync(new TaskItem
        {
            Title          = "Alt",
            Description    = "Alte Beschreibung",
            DueDate        = DateTime.Today.AddDays(3),
            EstimatedHours = 1,
            Status         = AppTaskStatus.Open
        });

        var updated = await service.UpdateAsync(created.Id, new TaskItem
        {
            Title          = "Neu",
            Description    = "Neue Beschreibung",
            DueDate        = DateTime.Today.AddDays(10),
            EstimatedHours = 5,
            Status         = AppTaskStatus.InProgress
        });

        Assert.IsNotNull(updated);
        Assert.AreEqual("Neu",                        updated.Title);
        Assert.AreEqual("Neue Beschreibung",          updated.Description);
        Assert.AreEqual(5.0,                          updated.EstimatedHours);
        Assert.AreEqual(AppTaskStatus.InProgress,     updated.Status);
    }

    [TestMethod]
    public async Task UpdateStatusAsync_NonExistentId_ReturnsNull()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.TaskItemService(ctx);

        var result = await service.UpdateStatusAsync(999, AppTaskStatus.Done);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task UpdateStatusAsync_ExistingItem_OnlyChangesStatus()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.TaskItemService(ctx);

        var created = await service.CreateAsync(new TaskItem
        {
            Title          = "Task",
            DueDate        = DateTime.Today.AddDays(5),
            EstimatedHours = 2,
            Status         = AppTaskStatus.Open
        });

        var updated = await service.UpdateStatusAsync(created.Id, AppTaskStatus.Done);

        Assert.IsNotNull(updated);
        Assert.AreEqual(AppTaskStatus.Done, updated.Status);
        Assert.AreEqual("Task", updated.Title);  // Titel unverändert
    }

    [TestMethod]
    public async Task DeleteAsync_NonExistentId_ReturnsFalse()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.TaskItemService(ctx);

        var result = await service.DeleteAsync(999);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task DeleteAsync_ExistingItem_ReturnsTrueAndRemovesItem()
    {
        using var ctx = DbContextFactory.Create();
        var service   = new Logic.Services.TaskItemService(ctx);

        var created = await service.CreateAsync(new TaskItem
        {
            Title          = "Zu löschen",
            DueDate        = DateTime.Today.AddDays(2),
            EstimatedHours = 1,
            Status         = AppTaskStatus.Open
        });

        var deleted = await service.DeleteAsync(created.Id);
        var found   = await service.GetByIdAsync(created.Id);

        Assert.IsTrue(deleted);
        Assert.IsNull(found);
    }
}
