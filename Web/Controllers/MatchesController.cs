using Data.Data;
using Data.Dto.CRUD.MatchRecord;
using Data.Models;
using Data.Services.Stores;
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

    [HttpGet("data")]
    public async Task<IActionResult> GetAll(string? search)
    {
        var records = await _store.GetMatchRecordsForTableAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            records = records
                .Where(record => (record.PlayingFieldName ?? string.Empty)
                    .Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Json(records);
    }

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

    [HttpGet("getById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var recordResult = await _store.FindByIdAsync(id);
        if (!recordResult.IsSuccess)
            return NotFound();

        return Json(recordResult.Value);
    }



    [HttpPost("create")]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] MatchRecordFormDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var record = new MatchRecord
        {
            Id = Guid.NewGuid(),
            WasMatchHeld = model.WasMatchHeld,
            MatchHeld = model.MatchHeld,
            PlayingFieldId = model.PlayingFieldId,
            GoalsTeamA = model.GoalsTeamA,
            GoalsTeamB = model.GoalsTeamB,
        };

        var result = await _store.CreateMatchRecord(record);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        var recordId = result.Value;
        await SyncMatchPlayers(recordId, model.MatchPlayerIds, model.MatchPlayerStats);
        await SyncPlayerRatings(recordId, model.PlayerRatings);

        return Ok();
    }


    [HttpPost("edit/{id:guid}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] MatchRecordFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var recordResult = await _store.FindByIdAsync(id);
        if (!recordResult.IsSuccess)
            return NotFound();

        var record = recordResult.Value;
        record.WasMatchHeld = model.WasMatchHeld;
        record.MatchHeld = model.MatchHeld;
        record.PlayingFieldId = model.PlayingFieldId;
        record.GoalsTeamA = model.GoalsTeamA;
        record.GoalsTeamB = model.GoalsTeamB;

        var result = await _store.UpdateMatchRecord(record);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        await SyncMatchPlayers(id, model.MatchPlayerIds, model.MatchPlayerStats);
        await SyncPlayerRatings(id, model.PlayerRatings);

        return Ok();
    }


    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        var recordResult = await _store.FindByIdAsync(id);
        if (!recordResult.IsSuccess)
            return NotFound();

        await _store.DeleteByIdAsync(id);
        return Ok();
    }

    private async Task SyncMatchPlayers(Guid recordId, List<Guid> matchPlayerIds, List<MatchPlayerStatsDto> stats)
    {
        var selectedIds = new HashSet<Guid>(matchPlayerIds);
        var statsById = stats.ToDictionary(s => s.PlayerId);
        var existingPlayers = await _matchPlayerStore.GetByMatchRecordIdAsync(recordId);
        var existingByPlayerId = existingPlayers.ToDictionary(player => player.PlayerId, player => player);

        foreach (var existing in existingPlayers)
        {
            if (!selectedIds.Contains(existing.PlayerId))
            {
                await _matchPlayerStore.DeleteByIdAsync(existing.Id);
                continue;
            }

            var s = statsById.GetValueOrDefault(existing.PlayerId);
            await _matchPlayerStore.UpdateStatsAsync(existing.Id, s?.Goals ?? 0, s?.Assists ?? 0);
        }

        foreach (var playerId in selectedIds)
        {
            if (existingByPlayerId.ContainsKey(playerId))
                continue;

            var s = statsById.GetValueOrDefault(playerId);
            var matchPlayer = new MatchPlayer
            {
                Id = Guid.NewGuid(),
                PlayerId = playerId,
                MatchRecordId = recordId,
                Team = Team.A,
                Goals = s?.Goals ?? 0,
                Assists = s?.Assists ?? 0,
            };

            await _matchPlayerStore.CreateMatchPlayer(matchPlayer);
        }
    }

    private async Task SyncPlayerRatings(Guid recordId, List<MatchPlayerRatingDto> ratings)
    {
        var matchPlayers = await _matchPlayerStore.GetByMatchRecordIdAsync(recordId);
        var matchPlayerIds = matchPlayers.Select(player => player.Id).ToList();
        var matchPlayersByPlayerId = matchPlayers.ToDictionary(player => player.PlayerId, player => player.Id);

        await _playerRatingStore.DeleteByMatchPlayerIdsAsync(matchPlayerIds);

        foreach (var rating in ratings)
        {
            if (!matchPlayersByPlayerId.TryGetValue(rating.PlayerReceivingRatingId, out var matchPlayerId))
            {
                continue;
            }

            var newRating = new PlayerRating
            {
                Id = Guid.NewGuid(),
                MatchPlayerId = matchPlayerId,
                PlayerGivingRatingId = rating.PlayerGivingRatingId,
                PlayerReceivingRatingId = rating.PlayerReceivingRatingId,
                Rating = rating.Rating,
            };

            await _playerRatingStore.CreatePlayerRating(newRating);
        }
    }
}
