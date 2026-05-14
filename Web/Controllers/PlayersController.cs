using Data.Data;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Route("players")]
public class PlayersController : Controller
{
    private readonly PlayerStore _store;

    public PlayersController(PlayerStore store) => _store = store;

    public async Task<IActionResult> Index()
    {
        var players = await _store.GetAllPlayersAsync();
        return View("Players", players);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var player = await _store.FindByIdAsync(id);
        if (player is null)
        {
            return NotFound();
        }
        return View("PlayerDetailsView", player);
    }
}