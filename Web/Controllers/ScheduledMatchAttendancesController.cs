using Data.Dto.CRUD.ScheduledMatchAttendance;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Route("attendance")]
public class ScheduledMatchAttendancesController : Controller
{
    private readonly ScheduledMatchAttendanceStore _store;

    public ScheduledMatchAttendancesController(ScheduledMatchAttendanceStore store) => _store = store;

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var attendances = await _store.GetAllScheduledMatchAttendancesAsync();
        return PartialView("~/Views/Dashboard/ScheduledMatchAttendances/_List.cshtml", attendances);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("~/Views/Dashboard/ScheduledMatchAttendances/_Form.cshtml", new ScheduledMatchAttendanceFormDto());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(ScheduledMatchAttendanceFormDto model)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/ScheduledMatchAttendances/_Form.cshtml", model);
        }

        var attendance = new ScheduledMatchAttendance
        {
            Id = Guid.NewGuid(),
            ScheduledMatchId = model.ScheduledMatchId,
            PlayerId = model.PlayerId,
            IsAttending = model.IsAttending,
        };

        var result = await _store.CreateScheduledMatchAttendance(attendance);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/ScheduledMatchAttendances/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var attendanceResult = await _store.FindByIdAsync(id);
        if (!attendanceResult.IsSuccess)
            return NotFound();

        var attendance = attendanceResult.Value;
        var model = new ScheduledMatchAttendanceFormDto
        {
            Id = attendance.Id,
            ScheduledMatchId = attendance.ScheduledMatchId,
            PlayerId = attendance.PlayerId,
            IsAttending = attendance.IsAttending,
        };

        return PartialView("~/Views/Dashboard/ScheduledMatchAttendances/_Form.cshtml", model);
    }

    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, ScheduledMatchAttendanceFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/ScheduledMatchAttendances/_Form.cshtml", model);
        }

        var attendanceResult = await _store.FindByIdAsync(id);
        if (!attendanceResult.IsSuccess)
            return NotFound();

        var attendance = attendanceResult.Value;
        attendance.ScheduledMatchId = model.ScheduledMatchId;
        attendance.PlayerId = model.PlayerId;
        attendance.IsAttending = model.IsAttending;

        var result = await _store.UpdateScheduledMatchAttendance(attendance);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/ScheduledMatchAttendances/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var attendanceResult = await _store.FindByIdAsync(id);
        if (!attendanceResult.IsSuccess)
            return NotFound();

        try
        {
            await _store.DeleteByIdAsync(id);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Cannot delete attendance record while related data exists.");
        }

        return Ok();
    }
}
