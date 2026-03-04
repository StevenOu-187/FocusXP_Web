// @GeneratedCode
using LearningManager.Logic.DataContext;
using LearningManager.Logic.Entities.App;
using Microsoft.EntityFrameworkCore;
using AppTaskStatus = LearningManager.Logic.Entities.App.TaskStatus;

namespace LearningManager.Logic.Services;

/// <summary>
/// CRUD service for TaskItem entities.
/// </summary>
public class TaskItemService
{
    private readonly ProjectDbContext _context;

    public TaskItemService(ProjectDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
        => await _context.TaskItems.AsNoTracking().OrderBy(t => t.DueDate).ToListAsync();

    public async Task<TaskItem?> GetByIdAsync(int id)
        => await _context.TaskItems.FindAsync(id);

    public async Task<TaskItem> CreateAsync(TaskItem item)
    {
        _context.TaskItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<TaskItem?> UpdateAsync(int id, TaskItem updated)
    {
        var existing = await _context.TaskItems.FindAsync(id);
        if (existing is null) return null;

        existing.Title          = updated.Title;
        existing.Description    = updated.Description;
        existing.DueDate        = updated.DueDate;
        existing.EstimatedHours = updated.EstimatedHours;
        existing.Status         = updated.Status;

        await _context.SaveChangesAsync();
        return existing;
    }

    /// <summary>Patch: update only the status of a task.</summary>
    public async Task<TaskItem?> UpdateStatusAsync(int id, AppTaskStatus status)
    {
        var existing = await _context.TaskItems.FindAsync(id);
        if (existing is null) return null;

        existing.Status = status;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.TaskItems.FindAsync(id);
        if (existing is null) return false;

        _context.TaskItems.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
