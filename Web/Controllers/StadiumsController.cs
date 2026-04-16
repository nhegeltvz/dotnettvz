using Data.Data;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class StadiumsController : Controller
{
    private readonly MockRepository _mockRepository;

    public StadiumsController(MockRepository mockRepository)
    {
        _mockRepository = mockRepository;
    }

    public async Task<IActionResult> Index()
    {
        var fields = await _mockRepository.GetPlayingFields();
        return View("StadiumsView", fields);
    }

    public async Task<IActionResult> Details(Guid id, string? name)
    {
        var fields = await _mockRepository.GetPlayingFields();

        Data.Models.PlayingField? field = null;

        if (!string.IsNullOrWhiteSpace(name))
        {
            field = fields.FirstOrDefault(f =>
                string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        field ??= fields.FirstOrDefault(f => f.Id == id);

        if (field is null)
        {
            return NotFound();
        }

        var matchRecords = await _mockRepository.GetMatchRecords();
        var playedMatchesCount = matchRecords.Count(m =>
            string.Equals(m.PlayingField.Name, field.Name, StringComparison.OrdinalIgnoreCase)
            && m.WasMatchHeld);

        var model = new StadiumDetailsViewModel
        {
            Field = field,
            PlayedMatchesCount = playedMatchesCount
        };

        return View("StadiumDetailsView", model);
    }
}
