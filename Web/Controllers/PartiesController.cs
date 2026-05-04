using Data.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Web.Controllers;

public class PartiesController : Controller
{
    private readonly MatchTrackerDbContext _dbContext;

    public PartiesController(MatchTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var parties = await _dbContext.Parties
            .Include(party => party.PlayerCreated)
            .Include(party => party.Members)
            .AsNoTracking()
            .ToListAsync();
        return View("PartiesView", parties);
    }

    public async Task<IActionResult> Details(Guid id, string? createdAt)
    {
        var query = _dbContext.Parties
            .Include(party => party.PlayerCreated)
            .Include(party => party.Members)
            .Include(party => party.PreferredPlayingDates)
            .AsNoTracking();

        Data.Models.Party? party = null;

        if (!string.IsNullOrWhiteSpace(createdAt)
            && DateTime.TryParse(
                createdAt,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var createdAtValue))
        {
            var dayStart = createdAtValue.Date;
            var dayEnd = dayStart.AddDays(1);
            party = await query.FirstOrDefaultAsync(p =>
                p.DateCreated >= dayStart && p.DateCreated < dayEnd);
        }

        party ??= await query.FirstOrDefaultAsync(p => p.Id == id);

        if (party is null)
        {
            return NotFound();
        }

        return View("PartyDetailsView", party);
    }
}
