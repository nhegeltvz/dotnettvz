using Data.Dto.CRUD.ScheduledMatch;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Models.Dashboard;

namespace Web.Controllers;

[Route("scheduled-matches")]
public class ScheduledMatchesController : Controller
{
    private readonly ScheduledMatchStore _store;
    private readonly StadiumStore _stadiumStore;
    private readonly PartyStore _partyStore;

    public ScheduledMatchesController(
        ScheduledMatchStore store,
        StadiumStore stadiumStore,
        PartyStore partyStore)
    {
        _store = store;
        _stadiumStore = stadiumStore;
        _partyStore = partyStore;
    }

    [HttpGet("data")]
    public async Task<IActionResult> GetAll(string? search)
    {
        var matches = await _store.GetScheduledMatchesForTableAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            matches = matches
                .Where(match =>
                    (match.PartyDescription ?? string.Empty)
                        .Contains(term, StringComparison.OrdinalIgnoreCase)
                    || (match.PlayingFieldName ?? string.Empty)
                        .Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Json(matches);
    }

    [HttpGet("form")]
    public async Task<IActionResult> Form()
    {
        var fields = await _stadiumStore.GetAllStadiumsAsync();
        var parties = await _partyStore.GetAllPartiesAsync();

        var vm = new ScheduledMatchFormViewModel
        {
            PlayingFields = fields.Select(field => new SelectListItem
            {
                Value = field.Id.ToString(),
                Text = field.Name,
            }).ToList(),
            Parties = parties.Select(party => new SelectListItem
            {
                Value = party.Id.ToString(),
                Text = party.PartyDescription,
            }).ToList()
        };

        return PartialView("_ScheduledMatchForm", vm);
    }

    [HttpPost("create")]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] ScheduledMatchFormDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var scheduledMatch = new ScheduledMatch
        {
            Id = Guid.NewGuid(),
            PlayingFieldId = model.PlayingFieldId,
            PartyId = model.PartyId,
            MatchDate = model.MatchDate,
        };

        var result = await _store.CreateScheduledMatch(scheduledMatch);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok();
    }

    [HttpPost("edit/{id:guid}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] ScheduledMatchFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var scheduledMatchResult = await _store.FindByIdAsync(id);
        if (!scheduledMatchResult.IsSuccess)
            return NotFound();

        var scheduledMatch = scheduledMatchResult.Value;
        scheduledMatch.PlayingFieldId = model.PlayingFieldId;
        scheduledMatch.PartyId = model.PartyId;
        scheduledMatch.MatchDate = model.MatchDate;

        var result = await _store.UpdateScheduledMatch(scheduledMatch);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _store.DeleteByIdAsync(id);
        return Ok();
    }
}
