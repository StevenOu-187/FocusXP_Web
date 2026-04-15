using LearningManager.Logic.DataContext;
using LearningManager.Logic.Entities.App;
using Microsoft.EntityFrameworkCore;

namespace LearningManager.Logic.Services;

/// <summary>
/// CRUD service for BlockedDay entities.
/// </summary>
public class BlockedDayService
{
    private readonly ProjectDbContext _context;

    public BlockedDayService(ProjectDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BlockedDay>> GetAllAsync()
        => await _context.BlockedDays.AsNoTracking().OrderBy(b => b.Date).ToListAsync();

    public async Task<IEnumerable<DateOnly>> GetBlockedDatesAsync(DateOnly from, DateOnly to)
        => await _context.BlockedDays
            .AsNoTracking()
            .Where(b => b.Date >= from && b.Date <= to)
            .Select(b => b.Date)
            .ToListAsync();

    public async Task<BlockedDay?> GetByDateAsync(DateOnly date)
        => await _context.BlockedDays.AsNoTracking().FirstOrDefaultAsync(b => b.Date == date);

    public async Task<BlockedDay> CreateAsync(BlockedDay blockedDay)
    {
        var existing = await _context.BlockedDays.FirstOrDefaultAsync(b => b.Date == blockedDay.Date);
        if (existing is not null)
        {
            // Update reason if already blocked
            existing.Reason = blockedDay.Reason;
            await _context.SaveChangesAsync();
            return existing;
        }

        _context.BlockedDays.Add(blockedDay);
        await _context.SaveChangesAsync();
        return blockedDay;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.BlockedDays.FindAsync(id);
        if (existing is null) return false;

        _context.BlockedDays.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByDateAsync(DateOnly date)
    {
        var existing = await _context.BlockedDays.FirstOrDefaultAsync(b => b.Date == date);
        if (existing is null) return false;

        _context.BlockedDays.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
