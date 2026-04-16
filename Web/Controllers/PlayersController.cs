using Data.Data;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class PlayersController : Controller
{
    private readonly MockRepository _mockRepository;

    public PlayersController(MockRepository mockRepository)
    {
        _mockRepository = mockRepository;
    }

    public async Task<IActionResult> Index()
    {
        var players = await _mockRepository.GetPlayers();
        return View("Players", players);
    }

    public async Task<IActionResult> Details(Guid id, string? username)
    {
        var players = await _mockRepository.GetPlayers();

        Data.Models.Player? player = null;

        if (!string.IsNullOrWhiteSpace(username))
        {
            player = players.FirstOrDefault(p =>
                string.Equals(p.Username, username, StringComparison.OrdinalIgnoreCase));
        }

        player ??= players.FirstOrDefault(p => p.Id == id);

        if (player is null)
        {
            return NotFound();
        }

        return View("PlayerDetailsView", player);
    }
}