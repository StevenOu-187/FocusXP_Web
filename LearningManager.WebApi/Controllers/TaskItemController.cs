// @GeneratedCode
using LearningManager.Logic.Entities.App;
using LearningManager.Logic.Services;
using LearningManager.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using AppTaskStatus = LearningManager.Logic.Entities.App.TaskStatus;

namespace LearningManager.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskItemController : ControllerBase
{
    private readonly TaskItemService _service;

    public TaskItemController(TaskItemService service)
    {
        _service = service;
    }

    // GET api/taskitem
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItemModel>>> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items.Select(ToModel));
    }

    // GET api/taskitem/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItemModel>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(ToModel(item));
    }

    // POST api/taskitem
    [HttpPost]
    public async Task<ActionResult<TaskItemModel>> Create([FromBody] TaskItemEditModel model)
    {
        var entity = ToEntity(model);
        var created = await _service.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToModel(created));
    }

    // PUT api/taskitem/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskItemModel>> Update(int id, [FromBody] TaskItemEditModel model)
    {
        var updated = await _service.UpdateAsync(id, ToEntity(model));
        return updated is null ? NotFound() : Ok(ToModel(updated));
    }

    // PATCH api/taskitem/5/status
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<TaskItemModel>> PatchStatus(int id, [FromBody] TaskStatusPatchModel patch)
    {
        if (!Enum.TryParse<AppTaskStatus>(patch.Status, ignoreCase: true, out var status))
            return BadRequest($"Unknown status '{patch.Status}'. Allowed: Open, InProgress, Done.");

        var updated = await _service.UpdateStatusAsync(id, status);
        return updated is null ? NotFound() : Ok(ToModel(updated));
    }

    // DELETE api/taskitem/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    // ── Mapping helpers ─────────────────────────────────────────────────────
    private static TaskItemModel ToModel(TaskItem t) => new()
    {
        Id              = t.Id,
        Title           = t.Title,
        Description     = t.Description,
        DueDate         = t.DueDate,
        EstimatedHours  = t.EstimatedHours,
        Status          = t.Status.ToString()
    };

    private static TaskItem ToEntity(TaskItemEditModel m)
    {
        Enum.TryParse<AppTaskStatus>(m.Status, ignoreCase: true, out var status);
        return new TaskItem
        {
            Title          = m.Title,
            Description    = m.Description,
            DueDate        = m.DueDate,
            EstimatedHours = m.EstimatedHours,
            Status         = status
        };
    }
}
