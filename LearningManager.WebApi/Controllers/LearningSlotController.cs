// @GeneratedCode
using LearningManager.Logic.Entities.App;
using LearningManager.Logic.Exceptions;
using LearningManager.Logic.Services;
using LearningManager.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearningManager.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LearningSlotController : ControllerBase
{
    private readonly LearningSlotService _service;

    public LearningSlotController(LearningSlotService service)
    {
        _service = service;
    }

    // GET api/learningslot
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LearningSlotModel>>> GetAll()
    {
        var slots = await _service.GetAllAsync();
        return Ok(slots.Select(ToModel));
    }

    // GET api/learningslot/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LearningSlotModel>> GetById(int id)
    {
        var slot = await _service.GetByIdAsync(id);
        return slot is null ? NotFound() : Ok(ToModel(slot));
    }

    // POST api/learningslot
    [HttpPost]
    public async Task<ActionResult<LearningSlotModel>> Create([FromBody] LearningSlotEditModel model)
    {
        try
        {
            var entity = ToEntity(model);
            var created = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToModel(created));
        }
        catch (LearningSlotValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT api/learningslot/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<LearningSlotModel>> Update(int id, [FromBody] LearningSlotEditModel model)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, ToEntity(model));
            return updated is null ? NotFound() : Ok(ToModel(updated));
        }
        catch (LearningSlotValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE api/learningslot/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    // ── Mapping helpers ─────────────────────────────────────────────────────
    private static LearningSlotModel ToModel(LearningSlot s) => new()
    {
        Id         = s.Id,
        DayOfWeek  = s.DayOfWeek,
        StartTime  = s.StartTime,
        EndTime    = s.EndTime
    };

    private static LearningSlot ToEntity(LearningSlotEditModel m) => new()
    {
        DayOfWeek = m.DayOfWeek,
        StartTime = m.StartTime,
        EndTime   = m.EndTime
    };
}
