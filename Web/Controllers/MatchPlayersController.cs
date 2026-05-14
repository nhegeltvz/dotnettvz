using Data.Dto.CRUD.MatchPlayer;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Route("match-players")]
public class MatchPlayersController : Controller
{
    private readonly MatchPlayerStore _store;

    public MatchPlayersController(MatchPlayerStore store) => _store = store;

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var matchPlayers = await _store.GetAllMatchPlayersAsync();
        return PartialView("~/Views/Dashboard/MatchPlayers/_List.cshtml", matchPlayers);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("~/Views/Dashboard/MatchPlayers/_Form.cshtml", new MatchPlayerFormDto());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(MatchPlayerFormDto model)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/MatchPlayers/_Form.cshtml", model);
        }

        var matchPlayer = new MatchPlayer
        {
            Id = Guid.NewGuid(),
            PlayerId = model.PlayerId,
            Team = model.Team,
            MatchRecordId = model.MatchRecordId,
            Goals = model.Goals,
            Assists = model.Assists,
        };

        var result = await _store.CreateMatchPlayer(matchPlayer);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/MatchPlayers/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var matchPlayerResult = await _store.FindByIdAsync(id);
        if (!matchPlayerResult.IsSuccess)
            return NotFound();

        var matchPlayer = matchPlayerResult.Value;
        var model = new MatchPlayerFormDto
        {
            Id = matchPlayer.Id,
            PlayerId = matchPlayer.PlayerId,
            Team = matchPlayer.Team,
            MatchRecordId = matchPlayer.MatchRecordId,
            Goals = matchPlayer.Goals,
            Assists = matchPlayer.Assists,
        };

        return PartialView("~/Views/Dashboard/MatchPlayers/_Form.cshtml", model);
    }

    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, MatchPlayerFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/MatchPlayers/_Form.cshtml", model);
        }

        var matchPlayerResult = await _store.FindByIdAsync(id);
        if (!matchPlayerResult.IsSuccess)
            return NotFound();

        var matchPlayer = matchPlayerResult.Value;
        matchPlayer.PlayerId = model.PlayerId;
        matchPlayer.Team = model.Team;
        matchPlayer.MatchRecordId = model.MatchRecordId;
        matchPlayer.Goals = model.Goals;
        matchPlayer.Assists = model.Assists;

        var result = await _store.UpdateMatchPlayer(matchPlayer);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/MatchPlayers/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var matchPlayerResult = await _store.FindByIdAsync(id);
        if (!matchPlayerResult.IsSuccess)
            return NotFound();

        try
        {
            await _store.DeleteByIdAsync(id);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Cannot delete match player while related data exists.");
        }

        return Ok();
    }
}
