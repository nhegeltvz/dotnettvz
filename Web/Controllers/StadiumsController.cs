using Data.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Controllers;

[Route("stadiums")]
public class StadiumsController : Controller
{
    private readonly MatchTrackerDbContext _dbContext;

    public StadiumsController(MatchTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    [HttpGet("list")]
    public async Task<IActionResult> Index()
    {
        var fields = await _dbContext.PlayingFields
            .AsNoTracking()
            .ToListAsync();
        return View("StadiumsView", fields);
    }

    [HttpGet("details/{id:guid}")]
    [HttpGet("details/by-name/{name}")]
    public async Task<IActionResult> Details(Guid id, string? name)
    {
        var query = _dbContext.PlayingFields
            .Include(field => field.MatchRecords)
            .AsNoTracking();

        Data.Models.PlayingField? field = null;

        if (!string.IsNullOrWhiteSpace(name))
        {
            var nameLower = name.ToLower();
            field = await query.FirstOrDefaultAsync(f => f.Name.ToLower() == nameLower);
        }

        field ??= await query.FirstOrDefaultAsync(f => f.Id == id);

        if (field is null)
        {
            return NotFound();
        }

        var playedMatchesCount = field.MatchRecords.Count(match => match.WasMatchHeld);

        var model = new StadiumDetailsViewModel
        {
            Field = field,
            PlayedMatchesCount = playedMatchesCount
        };

        return View("StadiumDetailsView", model);
    }
}
