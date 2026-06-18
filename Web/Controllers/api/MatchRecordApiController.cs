using Data.Data;
using Data.Data.Common;
using Data.Dto.CRUD.MatchRecord;
using Data.Dto.CRUD.PlayingField;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers.api
{
    [Route("api/match-records")]
    [ApiController]
    public class MatchRecordApiController : ControllerBase
    {
        private readonly MatchStore _matchStore;
        private readonly MatchPlayerStore _matchPlayerStore;
        private readonly PlayerRatingStore _playerRatingStore;
        public MatchRecordApiController(MatchStore matchStore, MatchPlayerStore matchPlayerStore, PlayerRatingStore playerRatingStore)
        {
            _matchStore = matchStore;
            _matchPlayerStore = matchPlayerStore;
            _playerRatingStore = playerRatingStore;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchRecordDto>>> Get([FromQuery] string? search)
        {
            var recordsQuery = _matchStore.QueryMatchesAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                recordsQuery = recordsQuery
                    .Where(record => (record.PlayingField.Name ?? string.Empty)
                        .Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            var matches = await recordsQuery
                .Select(MatchRecordDto.ToDto())
                .ToListAsync();

            return Ok(matches);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MatchRecordDto>> GetById(Guid id)
        {
            var entity = await _matchStore.FindByIdAsync(id);
            if (!entity.IsSuccess || entity.Value == null)
            {
                return NotFound();
            }

            return Ok(MatchRecordDto.ToDto().Compile()(entity.Value));
        }

        [Authorize(Roles = AppRoles.ADMIN_ROLE)]
        [HttpPost]
        public async Task<ActionResult<MatchRecordDto>> Post([FromBody] MatchRecordFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var matchRecord = new MatchRecord
            {
                Id = Guid.NewGuid(),
                WasMatchHeld = model.WasMatchHeld,
                MatchHeld = model.MatchHeld,
                PlayingFieldId = model.PlayingFieldId,
                GoalsTeamA = model.GoalsTeamA,
                GoalsTeamB = model.GoalsTeamB,
            };

            var result = await _matchStore.CreateMatchRecord(matchRecord);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var recordId = result.Value;
            await SyncMatchPlayers(recordId, model.MatchPlayerIds, model.MatchPlayerStats);
            await SyncPlayerRatings(recordId, model.PlayerRatings);

            var createdDto = MatchRecordDto.ToDto().Compile()(matchRecord);
            return CreatedAtAction(nameof(GetById), new { id = matchRecord.Id }, createdDto);
        }

        [Authorize(Roles = AppRoles.ADMIN_ROLE)]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MatchRecordDto>> Put(Guid id, [FromBody] MatchRecordFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var foundResult = await _matchStore.FindByIdAsync(id);
            if (!foundResult.IsSuccess || foundResult.Value == null)
                return NotFound();

            var matchRecord = foundResult.Value;
            matchRecord.WasMatchHeld = model.WasMatchHeld;
            matchRecord.MatchHeld = model.MatchHeld;
            matchRecord.PlayingFieldId = model.PlayingFieldId;
            matchRecord.GoalsTeamA = model.GoalsTeamA;
            matchRecord.GoalsTeamB = model.GoalsTeamB;

            var updateResult = await _matchStore.UpdateMatchRecord(matchRecord);
            if (!updateResult.IsSuccess)
                return BadRequest(updateResult.Errors);

            await SyncMatchPlayers(id, model.MatchPlayerIds, model.MatchPlayerStats);
            await SyncPlayerRatings(id, model.PlayerRatings);

            return NoContent();
        }

        [Authorize(Roles = AppRoles.ADMIN_ROLE)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _matchStore.DeleteByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound();
            }

            return NoContent();
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
}
