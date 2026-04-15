// @GeneratedCode
using LearningManager.Logic.Entities.App;
using Microsoft.EntityFrameworkCore;

namespace LearningManager.Logic.DataContext;

/// <summary>
/// EF Core DbContext for the LearningManager application.
/// </summary>
public class ProjectDbContext : DbContext
{
    public DbSet<LearningSlot> LearningSlots => Set<LearningSlot>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<BlockedDay> BlockedDays => Set<BlockedDay>();

    public ProjectDbContext(DbContextOptions<ProjectDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // LearningSlot configuration
        modelBuilder.Entity<LearningSlot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DayOfWeek).IsRequired();
            // SQLite stores TimeSpan as TEXT (ticks)
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.EndTime).IsRequired();
        });

        // TaskItem configuration
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.DueDate).IsRequired();
            entity.Property(e => e.EstimatedHours).IsRequired();
            entity.Property(e => e.Status).IsRequired()
                  .HasConversion<string>();
        });

        // BlockedDay configuration
        modelBuilder.Entity<BlockedDay>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.Reason).HasMaxLength(200);
            entity.HasIndex(e => e.Date).IsUnique();
        });
    }
}
