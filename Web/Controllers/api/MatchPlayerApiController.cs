using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers.api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MatchPlayerApiController : ControllerBase
{
    private readonly MatchPlayerStore _matchPlayerStore;
    private readonly PlayerRatingStore _playerRatingStore;
    private readonly PlayerStore _playerStore;
    private readonly UserManager<AppUser> _userManager;

    public MatchPlayerApiController(
        MatchPlayerStore matchPlayerStore,
        PlayerRatingStore playerRatingStore,
        PlayerStore playerStore,
        UserManager<AppUser> userManager)
    {
        _matchPlayerStore = matchPlayerStore;
        _playerRatingStore = playerRatingStore;
        _playerStore = playerStore;
        _userManager = userManager;
    }

    private async Task<Player?> GetCurrentPlayerAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return null;
        var result = await _playerStore.FindByUserIdAsync(user.Id);
        return result.IsSuccess ? result.Value : null;
    }

    // GET /api/matchplayer/my
    // Returns all MatchPlayer entries for the current user
    [HttpGet("my")]
    public async Task<IActionResult> MyMatches()
    {
        var player = await GetCurrentPlayerAsync();
        if (player == null) return Unauthorized();

        var matchPlayers = await _matchPlayerStore.GetByPlayerIdAsync(player.Id);
        return Ok(matchPlayers.Select(mp => new
        {
            matchPlayerId = mp.Id,
            matchRecordId = mp.MatchRecordId,
            team = mp.Team.ToString(),
            goals = mp.Goals,
            assists = mp.Assists,
            hasRated = mp.PlayerRating != null,
            matchHeld = mp.MatchRecord.MatchHeld,
            playingFieldName = mp.MatchRecord.PlayingField.Name,
            goalsTeamA = mp.MatchRecord.GoalsTeamA,
            goalsTeamB = mp.MatchRecord.GoalsTeamB,
        }));
    }

    // PUT /api/matchplayer/{id}/stats
    // Player updates their own goals + assists
    [HttpPut("{id:guid}/stats")]
    public async Task<IActionResult> UpdateStats(Guid id, [FromBody] UpdateStatsDto model)
    {
        var player = await GetCurrentPlayerAsync();
        if (player == null) return Unauthorized();

        var result = await _matchPlayerStore.FindByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        var mp = result.Value!;
        if (mp.PlayerId != player.Id) return Forbid();
        if (model.Goals < 0 || model.Assists < 0)
            return BadRequest(new { message = "Golovi i asistencije ne mogu biti negativni." });

        await _matchPlayerStore.UpdateStatsAsync(id, model.Goals, model.Assists);
        return NoContent();
    }

    // GET /api/matchplayer/{id}/rating-target
    // Returns a random other player to rate (or the already-assigned target if already picked)
    [HttpGet("{id:guid}/rating-target")]
    public async Task<IActionResult> GetRatingTarget(Guid id)
    {
        var player = await GetCurrentPlayerAsync();
        if (player == null) return Unauthorized();

        var result = await _matchPlayerStore.FindByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        var mp = result.Value!;
        if (mp.PlayerId != player.Id) return Forbid();
        if (mp.PlayerRating != null)
            return Ok(new { alreadyRated = true });

        // Load all match players for the same match, excluding self
        var allInMatch = await _matchPlayerStore.GetByMatchRecordIdAsync(mp.MatchRecordId);
        var candidates = allInMatch
            .Where(x => x.PlayerId != player.Id)
            .ToList();

        if (!candidates.Any())
            return Ok(new { alreadyRated = false, noTarget = true });

        // Pick deterministically random using player.Id XOR matchRecord.Id as seed
        // so the same player always gets the same target
        var seed = (mp.PlayerId.GetHashCode() ^ mp.MatchRecordId.GetHashCode()) & int.MaxValue;
        var target = candidates[seed % candidates.Count];

        // Load user nav for the target player
        var targetPlayerResult = await _playerStore.FindByIdAsync(target.PlayerId);
        var targetName = targetPlayerResult.IsSuccess
            ? targetPlayerResult.Value?.User?.UserName ?? "Unknown"
            : "Unknown";

        return Ok(new
        {
            alreadyRated = false,
            noTarget = false,
            targetMatchPlayerId = target.Id,
            targetPlayerId = target.PlayerId,
            targetPlayerName = targetName,
        });
    }

    // POST /api/matchplayer/{id}/rate
    // Submits the rating; id = the GIVER's MatchPlayer id
    [HttpPost("{id:guid}/rate")]
    public async Task<IActionResult> Rate(Guid id, [FromBody] SubmitRatingDto model)
    {
        var player = await GetCurrentPlayerAsync();
        if (player == null) return Unauthorized();

        if (model.Rating < 1 || model.Rating > 10)
            return BadRequest(new { message = "Ocjena mora biti između 1 i 10." });

        var result = await _matchPlayerStore.FindByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        var mp = result.Value!;
        if (mp.PlayerId != player.Id) return Forbid();
        if (mp.PlayerRating != null) return Conflict(new { message = "Već ste ocijenili igrača za ovaj meč." });

        // Verify the target is actually in the same match
        var allInMatch = await _matchPlayerStore.GetByMatchRecordIdAsync(mp.MatchRecordId);
        var targetMp = allInMatch.FirstOrDefault(x => x.Id == model.TargetMatchPlayerId);
        if (targetMp == null) return BadRequest(new { message = "Nevažeći ciljni igrač." });
        if (targetMp.PlayerId == player.Id) return BadRequest(new { message = "Ne možete ocijeniti sami sebe." });

        await _playerRatingStore.CreatePlayerRating(new PlayerRatingDto
        {
            MatchPlayerId = id,
            PlayerGivingRatingId = player.Id,
            PlayerReceivingRatingId = targetMp.PlayerId,
            Rating = model.Rating,
        });

        return NoContent();
    }
}

public record UpdateStatsDto(int Goals, int Assists);
public record SubmitRatingDto(Guid TargetMatchPlayerId, int Rating);

internal sealed class PlayerRatingDto : Data.Models.Interfaces.IPlayerRating
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MatchPlayerId { get; set; }
    public Guid PlayerGivingRatingId { get; set; }
    public Guid PlayerReceivingRatingId { get; set; }
    public int Rating { get; set; }
}
