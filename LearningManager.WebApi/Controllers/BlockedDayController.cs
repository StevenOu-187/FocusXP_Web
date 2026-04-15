using LearningManager.Logic.Entities.App;
using LearningManager.Logic.Services;
using LearningManager.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearningManager.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlockedDayController : ControllerBase
{
    private readonly BlockedDayService _service;

    public BlockedDayController(BlockedDayService service)
    {
        _service = service;
    }

    // GET api/blockedday
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlockedDayModel>>> GetAll()
    {
        var days = await _service.GetAllAsync();
        return Ok(days.Select(ToModel));
    }

    // POST api/blockedday
    [HttpPost]
    public async Task<ActionResult<BlockedDayModel>> Create([FromBody] BlockedDayEditModel model)
    {
        var entity = ToEntity(model);
        var created = await _service.CreateAsync(entity);
        return Ok(ToModel(created));
    }

    // DELETE api/blockedday/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    // DELETE api/blockedday/by-date/2026-03-10
    [HttpDelete("by-date/{date}")]
    public async Task<IActionResult> DeleteByDate(DateOnly date)
    {
        var deleted = await _service.DeleteByDateAsync(date);
        return deleted ? NoContent() : NotFound();
    }

    // ── Mapping helpers ──────────────────────────────────────────────────────
    private static BlockedDayModel ToModel(BlockedDay b) => new()
    {
        Id     = b.Id,
        Date   = b.Date,
        Reason = b.Reason
    };

    private static BlockedDay ToEntity(BlockedDayEditModel m) => new()
    {
        Date   = m.Date,
        Reason = m.Reason
    };
}
