using Data.Data.Common;
using Data.Dto.CRUD.PlayingField;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Controllers;

[Route("stadiums")]
public class StadiumsController : Controller
{
    private readonly StadiumStore _store;
    public StadiumsController(StadiumStore store)
    {
        _store = store;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var fields = await _store.GetAllStadiumsAsync();
        return View("StadiumsView", fields);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {


        var playingFieldResult = await _store.FindByIdAsync(id);

        if (playingFieldResult is null || !playingFieldResult.IsSuccess)
        {
            return NotFound();
        }

        var playingField = playingFieldResult.Value;

        var playedMatchesCount = playingField.MatchRecords.Count(match => match.WasMatchHeld);

        var model = new StadiumDetailsViewModel
        {
            Field = playingField,
            PlayedMatchesCount = playedMatchesCount
        };

        return View("StadiumDetailsView", model);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(string? search)
    {
        var fields = string.IsNullOrWhiteSpace(search)
            ? await _store.GetAllStadiumsAsync()
            : await _store.SearchByNameAsync(search);

        return PartialView("~/Views/Dashboard/Stadiums/_List.cshtml", fields);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("~/Views/Dashboard/Stadiums/_Form.cshtml", new StadiumFormDto());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(StadiumFormDto model)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Stadiums/_Form.cshtml", model);
        }


        var field = new PlayingField
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            Longitude = model.Longitude,
            Latitude = model.Latitude,
            ContactNumber = model.ContactNumber,
            Status = model.Status,
            IsOutdoor = model.IsOutdoor,
            SurfaceType = model.SurfaceType,
        };

        var result = await _store.CreatePlayingField(field);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Stadiums/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var fieldResult = await _store.FindByIdAsync(id);

        if (fieldResult is null || !fieldResult.IsSuccess )
        {
            return NotFound();
        }

        var field = fieldResult.Value;

        var model = new StadiumFormDto
        {
            Id = field.Id,
            Name = field.Name,
            Description = field.Description,
            Longitude = field.Longitude,
            Latitude = field.Latitude,
            ContactNumber = field.ContactNumber,
            Status = field.Status,
            IsOutdoor = field.IsOutdoor,
            SurfaceType = field.SurfaceType
        };

        return PartialView("~/Views/Dashboard/Stadiums/_Form.cshtml", model);
    }

    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, StadiumFormDto model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Stadiums/_Form.cshtml", model);
        }

        var fieldResult = await _store.FindByIdAsync(id);

        if (fieldResult is null || !fieldResult.IsSuccess)
        {
            return NotFound();
        }

        var field = fieldResult.Value;

        field.Name = model.Name;
        field.Description = model.Description;
        field.Longitude = model.Longitude;
        field.Latitude = model.Latitude;
        field.ContactNumber = model.ContactNumber;
        field.Status = model.Status;
        field.IsOutdoor = model.IsOutdoor;
        field.SurfaceType = model.SurfaceType;


        var result = await _store.UpdatePlayingField(field);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Stadiums/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var fieldResult = await _store.FindByIdAsync(id);
        if (fieldResult is null || !fieldResult.IsSuccess)
        {
            return NotFound();
        }
            var field = fieldResult.Value;

        try
        {
            await _store.DeleteByIdAsync(field.Id);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Cannot delete stadium while related data exists.");
        }

        return Ok();
    }
    [HttpGet("search")]
    public async Task<IActionResult> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return Ok(Array.Empty<string>());
        }

        var names = await _store.SearchByNameAsync(term);
        return Ok(names);
    }
}
