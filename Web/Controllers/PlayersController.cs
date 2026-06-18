using Data.Data;
using Data.Dto.CRUD.Player;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models.Dashboard;

namespace Web.Controllers;

[Route("players")]
public class PlayersController : Controller
{
    private readonly PlayerStore _store;
    private readonly UserManager<AppUser> _userManager;

    public PlayersController(PlayerStore store, UserManager<AppUser> userManager)
    {
        _store = store;
        _userManager = userManager;
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var playerResult = await _store.FindByIdAsync(id);
        if (!playerResult.IsSuccess)
            return NotFound();

        return View("PlayerDetailsView", playerResult.Value);
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var players = await _store.GetAllPlayersAsync();
        return View("Players", players);
    }

    [Authorize(Roles = AppRoles.ADMIN_ROLE)]
    [HttpGet("form")]
    public async Task<IActionResult> Form(Guid? id)
    {
        // Users that already have a player profile
        var takenUserIds = await _store.QueryPlayersAsync()
            .Select(p => p.UserId)
            .ToListAsync();

        // If editing, the current player's UserId should remain available
        if (id.HasValue)
        {
            var existing = await _store.FindByIdAsync(id.Value);
            if (existing.IsSuccess)
                takenUserIds.Remove(existing.Value!.UserId);
        }

        var availableUsers = await _userManager.Users
            .Where(u => !takenUserIds.Contains(u.Id))
            .OrderBy(u => u.UserName)
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.UserName ?? u.Email ?? u.Id.ToString(),
            })
            .ToListAsync();

        var vm = new PlayerFormViewModel { Users = availableUsers };

        if (id.HasValue)
        {
            var playerResult = await _store.FindByIdAsync(id.Value);
            if (!playerResult.IsSuccess)
                return NotFound();

            var player = playerResult.Value!;
            vm.Form.Id = player.Id;
            vm.Form.UserId = player.UserId;
            vm.Form.Bio = player.Bio;
            vm.Form.PreferredPosition = (int)player.PreferredPosition;
            vm.Form.DateOfBirth = player.DateOfBirth;
        }

        return PartialView("_PlayerForm", vm);
    }
}
