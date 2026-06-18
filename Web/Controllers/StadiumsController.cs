using Data.Data;
using Data.Dto.CRUD.PlayingField;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

[Route("stadiums")]
public class StadiumsController : Controller
{
    private readonly StadiumStore _store;

    public StadiumsController(StadiumStore store, IWebHostEnvironment env) => _store = store;

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

        var vm = new StadiumDetailsViewModel
        {
            Field = playingField,
            PlayedMatchesCount = playedMatchesCount
        };

        return View("StadiumDetailsView", vm);
    }

    [Authorize(Roles = AppRoles.ADMIN_ROLE)]
    [HttpGet("form")]
    public IActionResult Form() => PartialView("_StadiumForm", new StadiumFormDto());
}
