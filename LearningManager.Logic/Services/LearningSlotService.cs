// @GeneratedCode
using LearningManager.Logic.DataContext;
using LearningManager.Logic.Entities.App;
using LearningManager.Logic.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace LearningManager.Logic.Services;

/// <summary>
/// CRUD service for LearningSlot entities.
/// </summary>
public class LearningSlotService
{
    private readonly ProjectDbContext _context;

    public LearningSlotService(ProjectDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LearningSlot>> GetAllAsync()
    {
        var slots = await _context.LearningSlots.AsNoTracking().ToListAsync();
        return slots.OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime);
    }

    public async Task<LearningSlot?> GetByIdAsync(int id)
        => await _context.LearningSlots.FindAsync(id);

    public async Task<LearningSlot> CreateAsync(LearningSlot slot)
    {
        await ValidateSlotAsync(slot.DayOfWeek, slot.StartTime, slot.EndTime);
        _context.LearningSlots.Add(slot);
        await _context.SaveChangesAsync();
        return slot;
    }

    public async Task<LearningSlot?> UpdateAsync(int id, LearningSlot updated)
    {
        var existing = await _context.LearningSlots.FindAsync(id);
        if (existing is null) return null;

        await ValidateSlotAsync(updated.DayOfWeek, updated.StartTime, updated.EndTime, id);

        existing.DayOfWeek = updated.DayOfWeek;
        existing.StartTime = updated.StartTime;
        existing.EndTime   = updated.EndTime;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.LearningSlots.FindAsync(id);
        if (existing is null) return false;

        _context.LearningSlots.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task ValidateSlotAsync(int dayOfWeek, TimeSpan startTime, TimeSpan endTime, int? excludeId = null)
    {
        if (startTime >= endTime)
            throw new LearningSlotValidationException("Startzeit muss vor Endzeit liegen.");

        var daySlots = await _context.LearningSlots
            .AsNoTracking()
            .Where(s => s.DayOfWeek == dayOfWeek)
            .Where(s => !excludeId.HasValue || s.Id != excludeId.Value)
            .ToListAsync();

        var hasOverlap = daySlots.Any(s =>
            startTime < s.EndTime &&
            s.StartTime < endTime);

        if (hasOverlap)
            throw new LearningSlotValidationException("Für diesen Tag existiert bereits ein Lernblock mit gleicher oder überlappender Zeit.");
    }
}
