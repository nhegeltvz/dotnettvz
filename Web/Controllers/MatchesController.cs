using Data.Dto.CRUD.MatchRecord;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models.Dashboard;

namespace Web.Controllers;

[Route("matches")]
public class MatchesController : Controller
{
    private readonly MatchStore _store;
    private readonly StadiumStore _stadiumStore;

    public MatchesController(MatchStore store, StadiumStore stadiumStore)
    {
        _store = store;
        _stadiumStore = stadiumStore;
    }

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

    [HttpGet("data")]
    public async Task<IActionResult> GetAll()
    {
        var records = await _store.GetMatchRecordsForTableAsync();
        return Json(records);
    }

    [HttpGet("form")]
    public async Task<IActionResult> Form()
    {
        var stadiums = await _stadiumStore.GetAllStadiumsAsync();

        var vm = new MatchRecordFormViewModel
        {
            Stadiums = stadiums.Select(stadium => new SelectListItem
            {
                Value = stadium.Id.ToString(),
                Text = stadium.Name,
            }).ToList()
        };

        return PartialView("_MatchRecordForm", vm);
    }

    [HttpGet("getById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var recordResult = await _store.FindByIdAsync(id);
        if (!recordResult.IsSuccess)
            return NotFound();

        return Json(recordResult.Value);
    }



    [HttpPost("create")]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] MatchRecordFormDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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
            return BadRequest(result.Errors);

        return Ok();
    }


    [HttpPost("edit/{id:guid}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] MatchRecordFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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
            return BadRequest(result.Errors);

        return Ok();
    }


    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        var recordResult = await _store.FindByIdAsync(id);
        if (!recordResult.IsSuccess)
            return NotFound();

        await _store.DeleteByIdAsync(id);
        return Ok();
    }
}
