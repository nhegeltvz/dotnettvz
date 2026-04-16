using Data.Data;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Web.Controllers;

public class PartiesController : Controller
{
    private readonly MockRepository _mockRepository;

    public PartiesController(MockRepository mockRepository)
    {
        _mockRepository = mockRepository;
    }

    public async Task<IActionResult> Index()
    {
        var parties = await _mockRepository.GetParties();
        return View("PartiesView", parties);
    }

    public async Task<IActionResult> Details(Guid id, string? createdAt)
    {
        var parties = await _mockRepository.GetParties();

        Data.Models.Party? party = null;

        if (!string.IsNullOrWhiteSpace(createdAt)
            && DateTime.TryParse(
                createdAt,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var createdAtValue))
        {
            // Mock data IDs are regenerated on every fetch, so we match by createdAt.
            party = parties.FirstOrDefault(p => p.DateCreated.Date == createdAtValue.Date);
        }

        party ??= parties.FirstOrDefault(p => p.Id == id);

        if (party is null)
        {
            return NotFound();
        }

        return View("PartyDetailsView", party);
    }
}
