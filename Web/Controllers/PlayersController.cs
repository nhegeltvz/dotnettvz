using Data.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Route("players")]
public class PlayersController : Controller
{
    private readonly MatchTrackerDbContext _dbContext;

    public PlayersController(MatchTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var players = await _dbContext.Players
            .Include(player => player.MatchPlayers)
            .Include(player => player.RatingsReceived)
            .AsNoTracking()
            .ToListAsync();
        return View("Players", players);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id, string? username)
    {
        var query = _dbContext.Players
            .Include(player => player.MatchPlayers)
            .Include(player => player.RatingsReceived)
            .AsNoTracking();

        Data.Models.Player? player = null;

        if (!string.IsNullOrWhiteSpace(username))
        {
            var usernameLower = username.ToLower();
            player = await query.FirstOrDefaultAsync(p => p.Username.ToLower() == usernameLower);
        }

        player ??= await query.FirstOrDefaultAsync(p => p.Id == id);

        if (player is null)
        {
            return NotFound();
        }

        return View("PlayerDetailsView", player);
    }
}