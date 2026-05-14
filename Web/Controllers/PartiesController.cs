using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("parties")]
public class PartiesController : Controller
{
    private readonly PartyStore _store;
    public PartiesController(PartyStore store) => _store = store;
    public async Task<IActionResult> Index()
    {
        var parties = await _store.GetPartiesAsync();
        return View("PartiesView", parties);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var party = await _store.FindByIdAsync(id);
        if (party is null)
        {
            return NotFound();
        }
        return View("PartyDetailsView", party);
    }
}
