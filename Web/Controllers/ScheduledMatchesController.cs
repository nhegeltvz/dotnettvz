using Data.Dto.CRUD.ScheduledMatch;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Route("scheduled-matches")]
public class ScheduledMatchesController : Controller
{
    private readonly ScheduledMatchStore _store;

    public ScheduledMatchesController(ScheduledMatchStore store) => _store = store;

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var matches = await _store.GetAllScheduledMatchesAsync();
        return PartialView("~/Views/Dashboard/ScheduledMatches/_List.cshtml", matches);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("~/Views/Dashboard/ScheduledMatches/_Form.cshtml", new ScheduledMatchFormDto());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(ScheduledMatchFormDto model)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/ScheduledMatches/_Form.cshtml", model);
        }

        var scheduledMatch = new ScheduledMatch
        {
            Id = Guid.NewGuid(),
            PlayingFieldId = model.PlayingFieldId,
            PartyId = model.PartyId,
            MatchDate = model.MatchDate,
        };

        var result = await _store.CreateScheduledMatch(scheduledMatch);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/ScheduledMatches/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var scheduledMatchResult = await _store.FindByIdAsync(id);
        if (!scheduledMatchResult.IsSuccess)
            return NotFound();

        var scheduledMatch = scheduledMatchResult.Value;
        var model = new ScheduledMatchFormDto
        {
            Id = scheduledMatch.Id,
            PlayingFieldId = scheduledMatch.PlayingFieldId,
            PartyId = scheduledMatch.PartyId,
            MatchDate = scheduledMatch.MatchDate,
        };

        return PartialView("~/Views/Dashboard/ScheduledMatches/_Form.cshtml", model);
    }

    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, ScheduledMatchFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/ScheduledMatches/_Form.cshtml", model);
        }

        var scheduledMatchResult = await _store.FindByIdAsync(id);
        if (!scheduledMatchResult.IsSuccess)
            return NotFound();

        var scheduledMatch = scheduledMatchResult.Value;
        scheduledMatch.PlayingFieldId = model.PlayingFieldId;
        scheduledMatch.PartyId = model.PartyId;
        scheduledMatch.MatchDate = model.MatchDate;

        var result = await _store.UpdateScheduledMatch(scheduledMatch);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/ScheduledMatches/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var scheduledMatchResult = await _store.FindByIdAsync(id);
        if (!scheduledMatchResult.IsSuccess)
            return NotFound();

        try
        {
            await _store.DeleteByIdAsync(id);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Cannot delete scheduled match while related data exists.");
        }

        return Ok();
    }
}
