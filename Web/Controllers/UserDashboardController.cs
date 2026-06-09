using Data.Data;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Authorize]
[Route("user-dashboard")]
public class UserDashboardController(
    UserManager<AppUser> userManager,
    PlayerStore playerStore,
    PartyStore partyStore,
    StadiumStore stadiumStore,
    MatchPlayerStore matchPlayerStore) : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View();

    [HttpGet("my-parties")]
    public async Task<IActionResult> MyParties()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        // PlayerCreatedId now references Player.Id, find the player first
        var playerResult = await playerStore.FindByUserIdAsync(user.Id);
        if (!playerResult.IsSuccess || playerResult.Value == null)
            return View(new List<Party>());

        var parties = await partyStore.QueryPartiesAsync()
            .Where(p => p.PlayerCreatedId == playerResult.Value.Id)
            .OrderBy(p => p.DateCreated)
            .Take(3)
            .ToListAsync();

        var playingFields = await stadiumStore.GetAllStadiumsAsync();
        ViewBag.PlayingFields = playingFields;

        return View(parties);
    }

    [HttpGet("my-matches")]
    public async Task<IActionResult> MyMatches()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var playerResult = await playerStore.FindByUserIdAsync(user.Id);
        if (!playerResult.IsSuccess || playerResult.Value == null)
            return View(new List<MatchPlayer>());

        var matchPlayers = await matchPlayerStore.GetByPlayerIdAsync(playerResult.Value.Id);
        return View(matchPlayers);
    }

    [HttpPost("player-profile")]
    public async Task<IActionResult> UpdatePlayerProfile(string bio, Position preferredPosition, DateOnly dateOfBirth)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var playerResult = await playerStore.FindByUserIdAsync(user.Id);

        if (!playerResult.IsSuccess || playerResult.Value == null)
        {
            // No profile yet — create one
            var newPlayer = new Player
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Bio = bio ?? string.Empty,
                PreferredPosition = preferredPosition,
                DateOfBirth = dateOfBirth,
            };

            var createResult = await playerStore.CreatePlayer(newPlayer);
            if (!createResult.IsSuccess)
            {
                TempData["PlayerProfileError"] = string.Join(" ", createResult.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Index));
            }
        }
        else
        {
            var player = playerResult.Value;
            player.Bio = bio ?? string.Empty;
            player.PreferredPosition = preferredPosition;
            player.DateOfBirth = dateOfBirth;

            var updateResult = await playerStore.UpdatePlayer(player);
            if (!updateResult.IsSuccess)
            {
                TempData["PlayerProfileError"] = string.Join(" ", updateResult.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Index));
            }
        }

        TempData["PlayerProfileSuccess"] = "Profil igrača uspješno spremljen.";
        return RedirectToAction(nameof(Index));
    }
}
