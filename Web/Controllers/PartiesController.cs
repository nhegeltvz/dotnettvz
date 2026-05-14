using Data.Dto.CRUD.Party;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Route("parties")]
public class PartiesController : Controller
{
    private readonly PartyStore _store;

    public PartiesController(PartyStore store) => _store = store;

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

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var parties = await _store.GetAllPartiesAsync();
        return PartialView("~/Views/Dashboard/Parties/_List.cshtml", parties);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("~/Views/Dashboard/Parties/_Form.cshtml", new PartyFormDto());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(PartyFormDto model)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Parties/_Form.cshtml", model);
        }

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
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Parties/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var partyResult = await _store.FindByIdAsync(id);
        if (!partyResult.IsSuccess)
            return NotFound();

        var party = partyResult.Value;
        var model = new PartyFormDto
        {
            Id = party.Id,
            PlayerCreatedId = party.PlayerCreatedId,
            DateCreated = party.DateCreated,
            MaxMembers = party.MaxMembers,
            PartyDescription = party.PartyDescription,
            PreferredLocations = party.PreferredLocations,
        };

        return PartialView("~/Views/Dashboard/Parties/_Form.cshtml", model);
    }

    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, PartyFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Parties/_Form.cshtml", model);
        }

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
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Parties/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var partyResult = await _store.FindByIdAsync(id);
        if (!partyResult.IsSuccess)
            return NotFound();

        try
        {
            await _store.DeleteByIdAsync(id);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Cannot delete party while related data exists.");
        }

        return Ok();
    }
}
