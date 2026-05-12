using Data.Data;
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


        var playingField = await _store.FindByIdAsync(id);

        if (playingField is null)
        {
            return NotFound();
        }

        var playedMatchesCount = playingField.MatchRecords.Count(match => match.WasMatchHeld);

        var model = new StadiumDetailsViewModel
        {
            Field = playingField,
            PlayedMatchesCount = playedMatchesCount
        };

        return View("StadiumDetailsView", model);
    }
}
