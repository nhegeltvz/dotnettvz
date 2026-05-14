using Data.Data;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Web.Controllers;

[Route("matches")]
public class MatchesController : Controller
{
    private readonly MatchStore _store;

    public MatchesController(MatchStore store) => _store = store;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var matches = await _store.GetMatchesAsync();
        return View("MatchesView", matches);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var match = await _store.FindByIdAsync(id);

        if (match is null)
        {
            return NotFound();
        }

        return View("MatchDetailsView", match);
    }
}
