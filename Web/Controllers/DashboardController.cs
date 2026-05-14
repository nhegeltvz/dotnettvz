using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("dashboard")]
public class DashboardController : Controller
{
    private readonly PlayerStore _playerStore;

    public DashboardController(PlayerStore playerStore) => _playerStore = playerStore;

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("players")]
    public async Task<IActionResult> Players()
    {
        var players = await _playerStore.GetAllPlayersAsync();
        return PartialView("_PlayersList", players);
    }

    [HttpGet("players/create")]
    public IActionResult CreatePlayer()
    {
        return PartialView("_PlayerCreate");
    }

    [HttpGet("players/{id:guid}")]
    public async Task<IActionResult> EditPlayer(Guid id)
    {
        var player = await _playerStore.FindByIdAsync(id);
        if (player is null)
        {
            return NotFound();
        }

        return PartialView("_PlayerEdit", player);
    }
}
