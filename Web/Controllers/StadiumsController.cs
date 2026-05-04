using Data.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Controllers;

public class StadiumsController : Controller
{
    private readonly MatchTrackerDbContext _dbContext;

    public StadiumsController(MatchTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var fields = await _dbContext.PlayingFields
            .AsNoTracking()
            .ToListAsync();
        return View("StadiumsView", fields);
    }

    public async Task<IActionResult> Details(Guid id, string? name)
    {
        var query = _dbContext.PlayingFields
            .Include(field => field.MatchRecords)
            .AsNoTracking();

        Data.Models.PlayingField? field = null;

        if (!string.IsNullOrWhiteSpace(name))
        {
            field = await query.FirstOrDefaultAsync(f =>
                string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
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
