using Data.Data;
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
    [HttpGet("data")]
    public async Task<IActionResult> GetAll(string? search)
    {
        var playingFields = await _store.GetAllStadiumsAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            playingFields = playingFields
                .Where(field => (field.Name ?? string.Empty)
                    .Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Json(playingFields);
    }

    [HttpGet("form")]
    public IActionResult Form() => PartialView("_StadiumForm", new StadiumFormDto());

    [HttpGet("getById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var playingField = await _store.FindByIdAsync(id);
        return Json(playingField.Value);
    }

    [HttpPost("create")]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] StadiumFormDto playingFieldForm)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var playingField = new PlayingField
        {
            Id = Guid.NewGuid()
        };
        
        playingField.Name = playingFieldForm.Name;
        playingField.Description = playingFieldForm.Description;
        playingField.Longitude = playingFieldForm.Longitude;
        playingField.Latitude= playingFieldForm.Latitude;
        playingField.ContactNumber= playingFieldForm.ContactNumber;
        playingField.Status= (FieldStatus)playingFieldForm.Status;
        playingField.IsOutdoor= playingFieldForm.IsOutdoor;
        playingField.SurfaceType= (SurfaceType)playingFieldForm.SurfaceType;


        await _store.CreatePlayingField(playingField);
        return Ok();
    }

    [HttpPost("edit/{id:guid}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Edit([FromBody] StadiumFormDto playingFieldForm, Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var playingFieldResult = await _store.FindByIdAsync(id);

        if (playingFieldResult is null || !playingFieldResult.IsSuccess)
            return NotFound();

        var playingField = playingFieldResult.Value;

        playingField.Name = playingFieldForm.Name;
        playingField.Description = playingFieldForm.Description;
        playingField.Longitude = playingFieldForm.Longitude;
        playingField.Latitude = playingFieldForm.Latitude;
        playingField.ContactNumber = playingFieldForm.ContactNumber;
        playingField.Status = (FieldStatus)playingFieldForm.Status;
        playingField.IsOutdoor = playingFieldForm.IsOutdoor;
        playingField.SurfaceType = (SurfaceType)playingFieldForm.SurfaceType;


        await _store.UpdatePlayingField(playingField);
        return Ok();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _store.DeleteByIdAsync(id);
        return Ok();
    }

}
