using Data.Data;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class MatchesController : Controller
{
    private readonly MockRepository _mockRepository;

    public MatchesController(MockRepository mockRepository)
    {
        _mockRepository = mockRepository;
    }

    public async Task<IActionResult> Index()
    {
        var matches = await _mockRepository.GetMatchRecords();
        return View("MatchesView", matches);
    }

    public async Task<IActionResult> Details(Guid id, string? matchDate, string? fieldName)
    {
        var matches = await _mockRepository.GetMatchRecords();

        Data.Models.MatchRecord? match = null;

        if (!string.IsNullOrWhiteSpace(matchDate) && !string.IsNullOrWhiteSpace(fieldName))
        {
            match = matches.FirstOrDefault(m =>
                m.MatchHeld.Date.ToString("yyyy-MM-dd") == matchDate
                && string.Equals(m.PlayingField.Name, fieldName, StringComparison.OrdinalIgnoreCase));
        }

        match ??= matches.FirstOrDefault(m => m.Id == id);

        if (match is null)
        {
            return NotFound();
        }

        return View("MatchDetailsView", match);
    }
}
