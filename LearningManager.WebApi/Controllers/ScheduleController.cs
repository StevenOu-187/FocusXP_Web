// @GeneratedCode
using LearningManager.Logic.Services;
using LearningManager.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearningManager.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly SchedulingService _service;

    public ScheduleController(SchedulingService service)
    {
        _service = service;
    }

    /// <summary>
    /// GET /api/schedule?from=2026-03-02&amp;to=2026-03-31
    /// Also supports legacy: ?weekStart=2026-03-02  (maps to a 7-day range)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduleEntryModel>>> GetSchedule(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] DateTime? weekStart)
    {
        DateTime rangeFrom, rangeTo;

        if (from.HasValue && to.HasValue)
        {
            rangeFrom = from.Value;
            rangeTo   = to.Value;
        }
        else if (weekStart.HasValue)
        {
            rangeFrom = weekStart.Value;
            rangeTo   = weekStart.Value.AddDays(6);
        }
        else
        {
            return BadRequest("Provide either 'from' & 'to' or 'weekStart' query parameters.");
        }

        var entries = await _service.GetScheduleAsync(rangeFrom, rangeTo);

        var result = entries.Select(e => new ScheduleEntryModel
        {
            Date       = e.Date,
            StartTime  = e.StartTime,
            EndTime    = e.EndTime,
            TaskTitle  = e.TaskTitle,
            TaskItemId = e.TaskItemId
        });

        return Ok(result);
    }
}
