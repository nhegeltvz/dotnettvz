using Data.Dto.CRUD.Party;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models.Dashboard;

namespace Web.Controllers;

[Route("parties")]
public class PartiesController : Controller
{
    private readonly PartyStore _store;
    private readonly PlayerStore _playerStore;

    public PartiesController(PartyStore store, PlayerStore playerStore)
    {
        _store = store;
        _playerStore = playerStore;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var parties = await _store.GetAllPartiesAsync();
        return View("PartiesView", parties);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var partyResult = await _store.FindByIdAsync(id);
        if (!partyResult.IsSuccess)
            return NotFound();

        return View("PartyDetailsView", partyResult.Value);
    }


    [HttpGet("data")]
    public async Task<IActionResult> GetAll()
    {
        var parties = await _store.GetPartiesForTableAsync();
        return Json(parties);
    }

    [HttpGet("form")]
    public async Task<IActionResult> Form()
    {
        var players = await _playerStore.GetAllPlayersAsync();

        var vm = new PartyFormViewModel
        {
            Players = players.Select(player => new SelectListItem
            {
                Value = player.Id.ToString(),
                Text = player.Username,
            }).ToList()
        };

        return PartialView("_PartyForm", vm);
    }

    [HttpGet("getById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var partyResult = await _store.FindByIdAsync(id);
        if (!partyResult.IsSuccess)
            return NotFound();

        return Json(partyResult.Value);
    }


    [HttpPost("create")]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] PartyFormDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var party = new Party
        {
            Id = Guid.NewGuid(),
            PlayerCreatedId = model.PlayerCreatedId,
            DateCreated = model.DateCreated,
            MaxMembers = model.MaxMembers,
            PartyDescription = model.PartyDescription,
            PreferredLocations = model.PreferredLocations,
        };

        var result = await _store.CreateParty(party);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok();
    }



    [HttpPost("edit/{id:guid}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] PartyFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var partyResult = await _store.FindByIdAsync(id);
        if (!partyResult.IsSuccess)
            return NotFound();

        var party = partyResult.Value;
        party.PlayerCreatedId = model.PlayerCreatedId;
        party.DateCreated = model.DateCreated;
        party.MaxMembers = model.MaxMembers;
        party.PartyDescription = model.PartyDescription;
        party.PreferredLocations = model.PreferredLocations;

        var result = await _store.UpdateParty(party);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok();
    }


    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        var partyResult = await _store.FindByIdAsync(id);
        if (!partyResult.IsSuccess)
            return NotFound();

        await _store.DeleteByIdAsync(id);
        return Ok();
    }
}
