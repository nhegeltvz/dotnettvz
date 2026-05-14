using Data.Dto.CRUD.MatchRecord;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Route("matches")]
public class MatchesController : Controller
{
    private readonly MatchStore _store;

    public MatchesController(MatchStore store) => _store = store;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var matches = await _store.GetAllMatchRecordsAsync();
        return View("MatchesView", matches);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var matchResult = await _store.FindByIdAsync(id);
        if (!matchResult.IsSuccess)
            return NotFound();

        return View("MatchDetailsView", matchResult.Value);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var records = await _store.GetAllMatchRecordsAsync();
        return PartialView("~/Views/Dashboard/Matches/_List.cshtml", records);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("~/Views/Dashboard/Matches/_Form.cshtml", new MatchRecordFormDto());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(MatchRecordFormDto model)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Matches/_Form.cshtml", model);
        }

        var record = new MatchRecord
        {
            Id = Guid.NewGuid(),
            WasMatchHeld = model.WasMatchHeld,
            MatchHeld = model.MatchHeld,
            PlayingFieldId = model.PlayingFieldId,
            GoalsTeamA = model.GoalsTeamA,
            GoalsTeamB = model.GoalsTeamB,
        };

        var result = await _store.CreateMatchRecord(record);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Matches/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var recordResult = await _store.FindByIdAsync(id);
        if (!recordResult.IsSuccess)
            return NotFound();

        var record = recordResult.Value;
        var model = new MatchRecordFormDto
        {
            Id = record.Id,
            WasMatchHeld = record.WasMatchHeld,
            MatchHeld = record.MatchHeld,
            PlayingFieldId = record.PlayingFieldId,
            GoalsTeamA = record.GoalsTeamA,
            GoalsTeamB = record.GoalsTeamB,
        };

        return PartialView("~/Views/Dashboard/Matches/_Form.cshtml", model);
    }

    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, MatchRecordFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Matches/_Form.cshtml", model);
        }

        var recordResult = await _store.FindByIdAsync(id);
        if (!recordResult.IsSuccess)
            return NotFound();

        var record = recordResult.Value;
        record.WasMatchHeld = model.WasMatchHeld;
        record.MatchHeld = model.MatchHeld;
        record.PlayingFieldId = model.PlayingFieldId;
        record.GoalsTeamA = model.GoalsTeamA;
        record.GoalsTeamB = model.GoalsTeamB;

        var result = await _store.UpdateMatchRecord(record);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Matches/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var recordResult = await _store.FindByIdAsync(id);
        if (!recordResult.IsSuccess)
            return NotFound();

        try
        {
            await _store.DeleteByIdAsync(id);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Cannot delete match record while related data exists.");
        }

        return Ok();
    }
}
