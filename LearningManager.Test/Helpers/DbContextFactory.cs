using LearningManager.Logic.DataContext;
using Microsoft.EntityFrameworkCore;

namespace LearningManager.Test.Helpers;

/// <summary>
/// Erstellt einen frischen In-Memory-DbContext für jeden Testfall.
/// </summary>
internal static class DbContextFactory
{
    public static ProjectDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ProjectDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ProjectDbContext(options);
    }
}
