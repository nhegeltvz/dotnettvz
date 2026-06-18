using Data.Data;
using Data.Dto.CRUD.MatchRecord;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models.Dashboard;

namespace Web.Controllers;

[Route("matches")]
public class MatchesController : Controller
{
    private readonly MatchStore _store;
    private readonly StadiumStore _stadiumStore;
    private readonly PlayerStore _playerStore;
    private readonly MatchPlayerStore _matchPlayerStore;
    private readonly PlayerRatingStore _playerRatingStore;

    public MatchesController(
        MatchStore store,
        StadiumStore stadiumStore,
        PlayerStore playerStore,
        MatchPlayerStore matchPlayerStore,
        PlayerRatingStore playerRatingStore)
    {
        _store = store;
        _stadiumStore = stadiumStore;
        _playerStore = playerStore;
        _matchPlayerStore = matchPlayerStore;
        _playerRatingStore = playerRatingStore;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var matches = await _store.GetAllMatchRecordsAsync();
        return View("MatchesView", matches);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var matchResult = await _store.FindByIdAsync(id);
        if (!matchResult.IsSuccess)
            return NotFound();

        return View("MatchDetailsView", matchResult.Value);
    }

    [Authorize(Roles = AppRoles.ADMIN_ROLE)]
    [HttpGet("form")]
    public async Task<IActionResult> Form(Guid? id)
    {
        var stadiums = await _stadiumStore.GetAllStadiumsAsync();
        var players = await _playerStore.GetAllPlayersAsync();

        var vm = new MatchRecordFormViewModel
        {
            Stadiums = stadiums.Select(stadium => new SelectListItem
            {
                Value = stadium.Id.ToString(),
                Text = stadium.Name,
            }).ToList(),
            HmsMatchPlayers = new Models.HegelMultiSelectConfig
            {
                ControlId = "hms-match-players",
                Label = "Match players",
                Placeholder = "Ronaldo",
                FieldName = "matchPlayers",
                AvailableItems = players.Select(player => new Models.SelectableItem
                {
                    Id = player.Id.ToString(),
                    Name = player.User.UserName ?? string.Empty
                }).ToList()
            }
        };

        if (id.HasValue)
        {
            var recordResult = await _store.FindByIdAsync(id.Value);
            if (!recordResult.IsSuccess)
            {
                return NotFound();
            }

            var record = recordResult.Value;
            vm.Form.Id = record.Id;
            vm.Form.WasMatchHeld = record.WasMatchHeld;
            vm.Form.MatchHeld = record.MatchHeld;
            vm.Form.PlayingFieldId = record.PlayingFieldId;
            vm.Form.GoalsTeamA = record.GoalsTeamA;
            vm.Form.GoalsTeamB = record.GoalsTeamB;
            vm.Form.MatchPlayerIds = record.MatchPlayers.Select(player => player.PlayerId).ToList();
            vm.HmsMatchPlayers.SelectedItems = record.MatchPlayers.Select(player => new Models.SelectableItem
            {
                Id = player.PlayerId.ToString(),
                Name = player.Player.User.UserName ?? string.Empty
            }).ToList();

            vm.Form.PlayerRatings = record.MatchPlayers
                .Where(player => player.PlayerRating != null)
                .Select(player => new MatchPlayerRatingDto
                {
                    PlayerGivingRatingId = player.PlayerRating!.PlayerGivingRatingId,
                    PlayerReceivingRatingId = player.PlayerRating.PlayerReceivingRatingId,
                    Rating = player.PlayerRating.Rating,
                }).ToList();

            vm.Form.MatchPlayerStats = record.MatchPlayers
                .Select(player => new MatchPlayerStatsDto
                {
                    PlayerId = player.PlayerId,
                    Goals = player.Goals,
                    Assists = player.Assists,
                }).ToList();
        }

        return PartialView("_MatchRecordForm", vm);
    }
}
