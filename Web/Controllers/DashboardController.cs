using Data.Data;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("dashboard")]
[Authorize(Roles = AppRoles.ADMIN_ROLE)]
public class DashboardController : Controller
{
    private readonly PlayerStore _playerStore;

    public DashboardController(PlayerStore playerStore) => _playerStore = playerStore;

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("parties")]
    public IActionResult Parties()
    {
        return View("Parties");
    }

    [HttpGet("match-records")]
    public IActionResult MatchRecords()
    {
        return View("MatchRecords");
    }

    [HttpGet("players")]
    public IActionResult Players()
    {
        return View("Players");
    }

    [HttpGet("playing-fields")]
    public IActionResult PlayingFields()
    {
        return View("Stadiums");
    }
}
