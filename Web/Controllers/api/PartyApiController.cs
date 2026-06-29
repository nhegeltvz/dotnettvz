using Data.Data;
using Data.Dto;
using Data.Dto.CRUD.Party;
using Data.Dto.CRUD.PlayingField;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models.Dto;

namespace Web.Controllers.api
{
    [Route("api/parties")]
    [ApiController]
    public class PartyApiController : ControllerBase
    {
        private readonly PartyStore _store;
        private readonly ChatMessageStore _chatMessageStore;
        private readonly UserManager<AppUser> _userManager;
        private readonly PlayerStore _playerStore;
        private readonly ScheduledMatchStore _scheduledMatchStore;
        private readonly PreferredPlayingDateStore _preferredDateStore;
        private readonly ScheduledMatchAttendanceStore _attendanceStore;
        private readonly MatchStore _matchStore;
        private readonly MatchPlayerStore _matchPlayerStore;

        public PartyApiController(PartyStore store, UserManager<AppUser> userManager, PlayerStore playerStore, ScheduledMatchStore scheduledMatchStore, PreferredPlayingDateStore preferredDateStore, ScheduledMatchAttendanceStore attendanceStore, MatchStore matchStore, MatchPlayerStore matchPlayerStore, ChatMessageStore chatMessageStore)
        {
            _store = store;
            _playerStore = playerStore;
            _userManager = userManager;
            _scheduledMatchStore = scheduledMatchStore;
            _preferredDateStore = preferredDateStore;
            _attendanceStore = attendanceStore;
            _matchStore = matchStore;
            _matchPlayerStore = matchPlayerStore;
            _chatMessageStore = chatMessageStore;
        }

        // Helper — gets the Player for the currently authenticated AppUser
        private async Task<Player?> GetCurrentPlayerAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;
            var result = await _playerStore.FindByUserIdAsync(user.Id);
            return result.IsSuccess ? result.Value : null;
        }

        #region BasicCrud

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartyDto>>> Get(string? search)
        {
            var partiesQuery = _store.QueryPartiesAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                partiesQuery = partiesQuery
                    .Where(party =>
                        party.PartyDescription.ToLower().Contains(term)
                        || party.PlayerCreated.User.UserName.ToLower().Contains(term));
            }

            var parties = await partiesQuery
                .Select(PartyDto.ToDto())
                .ToListAsync();

            return Ok(parties);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PartyDto>> GetById(Guid id)
        {
            var result = await _store.FindByIdAsync(id);
            if (!result.IsSuccess || result.Value == null)
            {
                return NotFound();
            }

            return Ok(PartyDto.ToDto().Compile()(result.Value));
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("can-create")]
        public async Task<IActionResult> CanCreate()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var playerResult = await _playerStore.FindByUserIdAsync(user.Id);
            if (!playerResult.IsSuccess || playerResult.Value == null)
                return Ok(new { canCreate = false, message = "Molimo vas dovršite svoj igrački profil da biste kreirali grupu." });

            var player = playerResult.Value;

            if (string.IsNullOrWhiteSpace(player.Bio))
                return Ok(new { canCreate = false, message = "Molimo vas dovršite svoj igrački profil da biste kreirali grupu." });

            var alreadyOwns = await _store.QueryPartiesAsync()
                .AnyAsync(p => p.PlayerCreatedId == player.Id);

            if (alreadyOwns)
                return Ok(new { canCreate = false, message = "Već ste vlasnik jedne grupe. Zatvorite je prije otvaranja nove." });

            return Ok(new { canCreate = true, message = (string?)null });
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost("user-create")]
        public async Task<IActionResult> UserCreate([FromBody] UserCreatePartyDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var player = await GetCurrentPlayerAsync();
            if (player == null)
                return BadRequest(new { message = "Molimo vas dovršite svoj igrački profil da biste kreirali grupu." });

            var party = new Party
            {
                Id = Guid.NewGuid(),
                PlayerCreatedId = player.Id,
                DateCreated = DateTime.UtcNow,
                MaxMembers = model.MaxMembers,
                PartyDescription = model.PartyDescription,
                PreferredLocations = model.PreferredLocations,
            };

            var result = await _store.CreateParty(party);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var partyId = result.Value;
            await _store.SyncMembersAsync(partyId, [player.Id]);
            await SyncPreferredDates(partyId, model.PreferredPlayingDates);
            return CreatedAtAction(nameof(GetById), new { id = partyId }, new { id = partyId });
        }

        [Authorize(Roles = AppRoles.ADMIN_ROLE)]
        [HttpPost("admin-create")]
        [Consumes("application/json")]
        public async Task<IActionResult> AdminCreate([FromBody] PartyFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var party = new Party
            {
                Id = Guid.NewGuid(),
                PlayerCreatedId = model.PlayerCreatedId,
                DateCreated = model.DateCreated,
                MaxMembers = model.MaxMembers,
                PartyDescription = model.PartyDescription,
                PreferredLocations = model.PreferredLocations,
            };

            var result = await _store.CreateParty(party);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var partyId = result.Value;
            await _store.SyncMembersAsync(partyId, model.MemberIds);
            await SyncPreferredDates(partyId, model.PreferredPlayingDates);

            var scheduledMatchId = await SyncScheduledMatch(partyId, model);
            if (scheduledMatchId.HasValue)
            {
                await SyncAttendances(scheduledMatchId.Value, model.ScheduledMatchAttendances);
            }

            return CreatedAtAction(nameof(GetById), new { id = partyId }, new { id = partyId });
        }





        [Authorize(Roles = "User,Admin")]
        [HttpPut("user-edit/{partyId:guid}")]
        public async Task<IActionResult> UserEditParty(Guid partyId, [FromBody] UserCreatePartyDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var player = await GetCurrentPlayerAsync();
            if (player == null) return Unauthorized();

            var partyResult = await _store.FindByIdAsync(partyId);
            if (!partyResult.IsSuccess) return NotFound();
            if (partyResult.Value!.PlayerCreatedId != player.Id) return Forbid();

            var party = partyResult.Value;
            party.MaxMembers = model.MaxMembers;
            party.PartyDescription = model.PartyDescription;
            party.PreferredLocations = model.PreferredLocations;

            var result = await _store.UpdateParty(party);
            if (!result.IsSuccess) return BadRequest(result.Errors);

            await _preferredDateStore.DeleteByPartyIdAsync(partyId);
            foreach (var date in model.PreferredPlayingDates.Distinct().Where(d => d != default))
            {
                await _preferredDateStore.CreatePreferredPlayingDate(new PreferredPlayingDate
                {
                    Id = Guid.NewGuid(),
                    PartyId = partyId,
                    Date = date,
                });
            }

            return NoContent();
        }

        [Authorize(Roles = AppRoles.ADMIN_ROLE)]
        [HttpPut("admin-edit/{id:guid}")]
        [Consumes("application/json")]
        public async Task<IActionResult> AdminEditParty(Guid id, [FromBody] PartyFormDto model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var partyResult = await _store.FindByIdAsync(id);
            if (!partyResult.IsSuccess)
                return NotFound();

            var party = partyResult.Value;
            party.PlayerCreatedId = model.PlayerCreatedId;
            party.DateCreated = model.DateCreated;
            party.MaxMembers = model.MaxMembers;
            party.PartyDescription = model.PartyDescription;
            party.PreferredLocations = model.PreferredLocations;

            var result = await _store.UpdateParty(party);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            await _store.SyncMembersAsync(id, model.MemberIds);
            await SyncPreferredDates(id, model.PreferredPlayingDates);

            var scheduledMatchId = await SyncScheduledMatch(id, model);
            if (scheduledMatchId.HasValue)
            {
                await SyncAttendances(scheduledMatchId.Value, model.ScheduledMatchAttendances);
            }

            return NoContent();
        }



        [Authorize(Roles = "User,Admin")]
        [HttpDelete("{partyId:guid}/close")]
        public async Task<IActionResult> UserDeleteParty(Guid partyId)
        {
            var player = await GetCurrentPlayerAsync();
            if (player == null) return Unauthorized();

            var partyResult = await _store.FindByIdAsync(partyId);
            if (!partyResult.IsSuccess) return NotFound();
            if (partyResult.Value!.PlayerCreatedId != player.Id) return Forbid();

            var result = await _store.DeleteByIdAsync(partyId);
            if (!result.IsSuccess) return BadRequest(result.Errors);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _store.DeleteByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound();
            }

            return NoContent();
        }

        #endregion

        #region UserActions

        [Authorize(Roles = "User,Admin")]
        [HttpPut("{partyId:guid}/schedule/{matchId:guid}")]
        public async Task<IActionResult> UpdateScheduleMatch(Guid partyId, Guid matchId, [FromBody] ScheduleMatchDto model)
        {
            var player = await GetCurrentPlayerAsync();
            if (player == null) return Unauthorized();

            var partyResult = await _store.FindByIdAsync(partyId);
            if (!partyResult.IsSuccess) return NotFound();
            if (partyResult.Value!.PlayerCreatedId != player.Id) return Forbid();

            var matchResult = await _scheduledMatchStore.FindByIdAsync(matchId);
            if (!matchResult.IsSuccess) return NotFound();

            var match = matchResult.Value!;
            match.PlayingFieldId = model.PlayingFieldId;
            match.MatchDate = model.MatchDate;

            var result = await _scheduledMatchStore.UpdateScheduledMatch(match);
            if (!result.IsSuccess) return BadRequest(result.Errors);

            return NoContent();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost("{partyId:guid}/attendance")]
        public async Task<IActionResult> ToggleAttendance(Guid partyId, [FromBody] AttendanceToggleDto model)
        {
            var player = await GetCurrentPlayerAsync();
            if (player == null) return Unauthorized();

            var partyResult = await _store.FindByIdAsync(partyId);
            if (!partyResult.IsSuccess) return NotFound();

            var party = partyResult.Value!;
            if (!party.Members.Any(m => m.Id == player.Id)) return Forbid();
            if (party.ScheduledMatch == null) return BadRequest(new { message = "Nema zakazanog meča." });

            var matchId = party.ScheduledMatch.Id;
            var existing = await _attendanceStore.FindByPlayerAndMatchAsync(player.Id, matchId);

            if (existing != null)
            {
                existing.IsAttending = model.IsAttending;
                await _attendanceStore.UpdateScheduledMatchAttendance(existing);
            }
            else
            {
                await _attendanceStore.CreateScheduledMatchAttendance(new ScheduledMatchAttendance
                {
                    Id = Guid.NewGuid(),
                    ScheduledMatchId = matchId,
                    PlayerId = player.Id,
                    IsAttending = model.IsAttending,
                });
            }

            return Ok();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost("{partyId:guid}/schedule")]
        public async Task<IActionResult> ScheduleMatch(Guid partyId, [FromBody] ScheduleMatchDto model)
        {
            var player = await GetCurrentPlayerAsync();
            if (player == null) return Unauthorized();

            var partyResult = await _store.FindByIdAsync(partyId);
            if (!partyResult.IsSuccess) return NotFound();
            if (partyResult.Value!.PlayerCreatedId != player.Id) return Forbid();

            var match = new ScheduledMatch
            {
                Id = Guid.NewGuid(),
                PartyId = partyId,
                PlayingFieldId = model.PlayingFieldId,
                MatchDate = model.MatchDate,
            };

            var result = await _scheduledMatchStore.CreateScheduledMatch(match);
            if (!result.IsSuccess) return BadRequest(result.Errors);

            return Ok(new { id = result.Value });
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost("{partyId:guid}/join")]
        public async Task<IActionResult> JoinParty(Guid partyId)
        {
            var player = await GetCurrentPlayerAsync();
            if (player == null)
                return BadRequest(new { message = "Molimo vas dovršite svoj igrački profil da biste se pridružili grupi." });

            if (string.IsNullOrWhiteSpace(player.Bio))
                return BadRequest(new { message = "Molimo vas dovršite svoj igrački profil da biste se pridružili grupi." });

            var partyResult = await _store.FindByIdAsync(partyId);
            if (!partyResult.IsSuccess) return NotFound();

            var party = partyResult.Value!;
            if (party.Members.Any(m => m.Id == player.Id))
                return Conflict(new { message = "Već ste član ove grupe." });

            if (party.Members.Count >= party.MaxMembers)
                return BadRequest(new { message = "Grupa je popunjena." });

            var newIds = party.Members.Select(m => m.Id).Append(player.Id).ToList();
            await _store.SyncMembersAsync(partyId, newIds);
            return Ok();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpDelete("{partyId:guid}/leave")]
        public async Task<IActionResult> LeaveParty(Guid partyId)
        {
            var player = await GetCurrentPlayerAsync();
            if (player == null) return Unauthorized();

            var partyResult = await _store.FindByIdAsync(partyId);
            if (!partyResult.IsSuccess) return NotFound();

            var party = partyResult.Value!;
            if (party.PlayerCreatedId == player.Id)
                return BadRequest(new { message = "Vlasnik ne može napustiti grupu." });

            var remaining = party.Members.Where(m => m.Id != player.Id).Select(m => m.Id).ToList();
            await _store.SyncMembersAsync(partyId, remaining);

            if (party.ScheduledMatch != null)
                await _attendanceStore.DeleteByPlayerAndMatchAsync(player.Id, party.ScheduledMatch.Id);

            return Ok();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpDelete("{partyId:guid}/kick/{memberId:guid}")]
        public async Task<IActionResult> KickMember(Guid partyId, Guid memberId)
        {
            var player = await GetCurrentPlayerAsync();
            if (player == null) return Unauthorized();

            var partyResult = await _store.FindByIdAsync(partyId);
            if (!partyResult.IsSuccess) return NotFound();
            if (partyResult.Value!.PlayerCreatedId != player.Id) return Forbid();

            var party = partyResult.Value!;
            var remaining = party.Members.Where(m => m.Id != memberId).Select(m => m.Id).ToList();
            await _store.SyncMembersAsync(partyId, remaining);

            if (party.ScheduledMatch != null)
                await _attendanceStore.DeleteByPlayerAndMatchAsync(memberId, party.ScheduledMatch.Id);

            return NoContent();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost("{partyId:guid}/finalize-match")]
        public async Task<IActionResult> FinalizeMatch(Guid partyId, [FromBody] FinalizeMatchDto model)
        {
            var player = await GetCurrentPlayerAsync();
            if (player == null) return Unauthorized();

            var partyResult = await _store.FindByIdAsync(partyId);
            if (!partyResult.IsSuccess) return NotFound();

            var party = partyResult.Value!;
            if (party.PlayerCreatedId != player.Id) return Forbid();
            if (party.ScheduledMatch == null) return BadRequest(new { message = "Nije zakazan meč za ovu grupu." });
            if (party.ScheduledMatch.MatchRecord != null) return Conflict(new { message = "Meč je već evidentiran." });

            if (model.GoalsTeamA < 0 || model.GoalsTeamB < 0)
                return BadRequest(new { message = "Rezultat ne može biti negativan." });
            if (!model.Players.Any())
                return BadRequest(new { message = "Morate dodijeliti barem jednog igrača ekipama." });

            // Create the MatchRecord
            var matchRecord = new MatchRecord
            {
                Id = Guid.NewGuid(),
                ScheduledMatchId = party.ScheduledMatch.Id,
                PlayingFieldId = party.ScheduledMatch.PlayingFieldId,
                WasMatchHeld = true,
                MatchHeld = DateTime.UtcNow,
                GoalsTeamA = model.GoalsTeamA,
                GoalsTeamB = model.GoalsTeamB,
            };

            var createResult = await _matchStore.CreateMatchRecord(matchRecord);
            if (!createResult.IsSuccess) return BadRequest(createResult.Errors);

            var matchRecordId = createResult.Value;

            // Create MatchPlayer for every assigned player
            foreach (var assignment in model.Players)
            {
                await _matchPlayerStore.CreateMatchPlayer(new MatchPlayerDto
                {
                    PlayerId = assignment.PlayerId,
                    Team = assignment.Team,
                    MatchRecordId = matchRecordId,
                    Goals = 0,
                    Assists = 0,
                });
            }

            return Ok(new { matchRecordId });
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPut("{partyId:guid}/finalize-match")]
        public async Task<IActionResult> UpdateFinalizedMatch(Guid partyId, [FromBody] FinalizeMatchDto model)
        {
            var player = await GetCurrentPlayerAsync();
            if (player == null) return Unauthorized();

            var partyResult = await _store.FindByIdAsync(partyId);
            if (!partyResult.IsSuccess) return NotFound();

            var party = partyResult.Value!;
            if (party.PlayerCreatedId != player.Id) return Forbid();
            if (party.ScheduledMatch?.MatchRecord == null)
                return BadRequest(new { message = "Nema evidentiranog rezultata za ažuriranje." });

            if (model.GoalsTeamA < 0 || model.GoalsTeamB < 0)
                return BadRequest(new { message = "Rezultat ne može biti negativan." });
            if (!model.Players.Any())
                return BadRequest(new { message = "Morate dodijeliti barem jednog igrača ekipama." });

            var matchRecord = party.ScheduledMatch.MatchRecord;
            matchRecord.GoalsTeamA = model.GoalsTeamA;
            matchRecord.GoalsTeamB = model.GoalsTeamB;

            var updateResult = await _matchStore.UpdateMatchRecord(matchRecord);
            if (!updateResult.IsSuccess) return BadRequest(updateResult.Errors);

            // Replace all match players
            await _matchPlayerStore.DeleteByMatchRecordIdAsync(matchRecord.Id);
            foreach (var assignment in model.Players)
            {
                await _matchPlayerStore.CreateMatchPlayer(new MatchPlayerDto
                {
                    PlayerId = assignment.PlayerId,
                    Team = assignment.Team,
                    MatchRecordId = matchRecord.Id,
                    Goals = 0,
                    Assists = 0,
                });
            }

            return Ok(new { matchRecordId = matchRecord.Id });
        }

        #endregion

        #region Messaging

        [HttpGet("{partyId:guid}/chat")]
        public async Task<IActionResult> GetChatHistory(Guid partyId, [FromQuery] int skip = 0)
        {
            var chatMessagesQuery = _chatMessageStore.QueryPartyChatHistory(partyId, skip: skip);

            var chatMessages = await chatMessagesQuery
                .Select(ChatMessageDto.ToDto())
                .ToListAsync();

            return Ok(chatMessages.OrderBy(m => m.SentAt));
        }

        #endregion


        private async Task SyncPreferredDates(Guid partyId, List<DateTime> preferredDates)
        {
            await _preferredDateStore.DeleteByPartyIdAsync(partyId);

            foreach (var date in preferredDates.Distinct())
            {
                if (date == default)
                {
                    continue;
                }
                var preferredDate = new PreferredPlayingDate
                {
                    Id = Guid.NewGuid(),
                    PartyId = partyId,
                    Date = date,
                };

                await _preferredDateStore.CreatePreferredPlayingDate(preferredDate);
            }
        }



        private async Task<Guid?> SyncScheduledMatch(Guid partyId, PartyFormDto model)
        {
            if (!model.ScheduledMatchPlayingFieldId.HasValue || !model.ScheduledMatchDate.HasValue)
            {
                return null;
            }

            if (model.ScheduledMatchId.HasValue)
            {
                var scheduledMatchResult = await _scheduledMatchStore.FindByIdAsync(model.ScheduledMatchId.Value);
                if (!scheduledMatchResult.IsSuccess)
                {
                    return null;
                }

                var scheduledMatch = scheduledMatchResult.Value;
                scheduledMatch.PlayingFieldId = model.ScheduledMatchPlayingFieldId.Value;
                scheduledMatch.PartyId = partyId;
                scheduledMatch.MatchDate = model.ScheduledMatchDate.Value;

                var updateResult = await _scheduledMatchStore.UpdateScheduledMatch(scheduledMatch);
                return updateResult.IsSuccess ? scheduledMatch.Id : null;
            }

            var newScheduledMatch = new ScheduledMatch
            {
                Id = Guid.NewGuid(),
                PlayingFieldId = model.ScheduledMatchPlayingFieldId.Value,
                PartyId = partyId,
                MatchDate = model.ScheduledMatchDate.Value,
            };

            var createResult = await _scheduledMatchStore.CreateScheduledMatch(newScheduledMatch);
            return createResult.IsSuccess ? createResult.Value : null;
        }

        private async Task SyncAttendances(Guid scheduledMatchId, List<PartyScheduledMatchAttendanceDto> attendances)
        {
            await _attendanceStore.DeleteByScheduledMatchIdAsync(scheduledMatchId);

            foreach (var attendance in attendances)
            {
                var scheduledAttendance = new ScheduledMatchAttendance
                {
                    Id = Guid.NewGuid(),
                    ScheduledMatchId = scheduledMatchId,
                    PlayerId = attendance.PlayerId,
                    IsAttending = attendance.IsAttending,
                };

                await _attendanceStore.CreateScheduledMatchAttendance(scheduledAttendance);
            }
        }
    }
}

public record FinalizeMatchDto(int GoalsTeamA, int GoalsTeamB, List<PlayerTeamAssignment> Players);
public record PlayerTeamAssignment(Guid PlayerId, Data.Data.Team Team);

// Inline IMatchPlayer impl used for MatchPlayerStore.CreateMatchPlayer
internal sealed class MatchPlayerDto : Data.Models.Interfaces.IMatchPlayer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlayerId { get; set; }
    public Data.Data.Team Team { get; set; }
    public Guid MatchRecordId { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
}
