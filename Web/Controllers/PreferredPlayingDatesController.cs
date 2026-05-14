using Data.Dto.CRUD.PreferredPlayingDate;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Route("preferred-dates")]
public class PreferredPlayingDatesController : Controller
{
    private readonly PreferredPlayingDateStore _store;

    public PreferredPlayingDatesController(PreferredPlayingDateStore store) => _store = store;

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var dates = await _store.GetAllPreferredPlayingDatesAsync();
        return PartialView("~/Views/Dashboard/PreferredPlayingDates/_List.cshtml", dates);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("~/Views/Dashboard/PreferredPlayingDates/_Form.cshtml", new PreferredPlayingDateFormDto());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(PreferredPlayingDateFormDto model)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/PreferredPlayingDates/_Form.cshtml", model);
        }

        var date = new PreferredPlayingDate
        {
            Id = Guid.NewGuid(),
            PartyId = model.PartyId,
            Date = model.Date,
        };

        var result = await _store.CreatePreferredPlayingDate(date);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/PreferredPlayingDates/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var dateResult = await _store.FindByIdAsync(id);
        if (!dateResult.IsSuccess)
            return NotFound();

        var date = dateResult.Value;
        var model = new PreferredPlayingDateFormDto
        {
            Id = date.Id,
            PartyId = date.PartyId,
            Date = date.Date,
        };

        return PartialView("~/Views/Dashboard/PreferredPlayingDates/_Form.cshtml", model);
    }

    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, PreferredPlayingDateFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/PreferredPlayingDates/_Form.cshtml", model);
        }

        var dateResult = await _store.FindByIdAsync(id);
        if (!dateResult.IsSuccess)
            return NotFound();

        var date = dateResult.Value;
        date.PartyId = model.PartyId;
        date.Date = model.Date;

        var result = await _store.UpdatePreferredPlayingDate(date);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/PreferredPlayingDates/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var dateResult = await _store.FindByIdAsync(id);
        if (!dateResult.IsSuccess)
            return NotFound();

        try
        {
            await _store.DeleteByIdAsync(id);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Cannot delete preferred playing date while related data exists.");
        }

        return Ok();
    }
}
