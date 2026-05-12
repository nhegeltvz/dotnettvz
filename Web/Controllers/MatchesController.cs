using Data.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Web.Controllers;

[Route("matches")]
public class MatchesController : Controller
{
    private readonly MatchTrackerDbContext _dbContext;

    public MatchesController(MatchTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var matches = await _dbContext.MatchRecords
            .Include(match => match.PlayingField)
            .AsNoTracking()
            .ToListAsync();
        return View("MatchesView", matches);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id, string? matchDate, string? fieldName)
    {
        var query = _dbContext.MatchRecords
            .Include(match => match.PlayingField)
            .Include(match => match.MatchPlayers)
                .ThenInclude(matchPlayer => matchPlayer.Player)
            .Include(match => match.MatchVotes)
                .ThenInclude(matchVote => matchVote.Player)
            .AsNoTracking();

        Data.Models.MatchRecord? match = null;

        if (!string.IsNullOrWhiteSpace(matchDate)
            && !string.IsNullOrWhiteSpace(fieldName)
            && DateTime.TryParseExact(
                matchDate,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var matchDateValue))
        {
            var dayStart = matchDateValue.Date;
            var dayEnd = dayStart.AddDays(1);
            var fieldNameLower = fieldName.ToLower();
            match = await query.FirstOrDefaultAsync(m =>
                m.MatchHeld >= dayStart
                && m.MatchHeld < dayEnd
                && m.PlayingField.Name.ToLower() == fieldNameLower);
        }

        match ??= await query.FirstOrDefaultAsync(m => m.Id == id);

        if (match is null)
        {
            return NotFound();
        }

        return View("MatchDetailsView", match);
    }
}
