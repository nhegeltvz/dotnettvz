using Data.Dto.CRUD.Party;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models.Dashboard;
using Web.Models.Dto;

namespace Web.Controllers;

[Route("parties")]
public class PartiesController : Controller
{
    private readonly PartyStore _store;
    private readonly PlayerStore _playerStore;
    private readonly StadiumStore _stadiumStore;
    private readonly PreferredPlayingDateStore _preferredDateStore;
    private readonly ScheduledMatchStore _scheduledMatchStore;
    private readonly ScheduledMatchAttendanceStore _attendanceStore;

    public PartiesController(
        PartyStore store,
        PlayerStore playerStore,
        StadiumStore stadiumStore,
        PreferredPlayingDateStore preferredDateStore,
        ScheduledMatchStore scheduledMatchStore,
        ScheduledMatchAttendanceStore attendanceStore)
    {
        _store = store;
        _playerStore = playerStore;
        _stadiumStore = stadiumStore;
        _preferredDateStore = preferredDateStore;
        _scheduledMatchStore = scheduledMatchStore;
        _attendanceStore = attendanceStore;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var parties = await _store.GetAllPartiesAsync();
        return View("PartiesView", parties);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var partyResult = await _store.FindByIdAsync(id);
        if (!partyResult.IsSuccess)
            return NotFound();

        return View("PartyDetailsView", partyResult.Value);
    }


    [HttpGet("data")]
    public async Task<IActionResult> GetAll(string? search)
    {
        var parties = await _store.GetPartiesForTableAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            parties = parties
                .Where(party =>
                    (party.PartyDescription ?? string.Empty)
                        .Contains(term, StringComparison.OrdinalIgnoreCase)
                    || (party.PlayerCreatedUsername ?? string.Empty)
                        .Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Json(parties);
    }

    [HttpGet("form")]
    public async Task<IActionResult> Form(Guid? id)
    {
        var players = await _playerStore.GetAllPlayersAsync();
        var fields = await _stadiumStore.GetAllStadiumsAsync();

        var vm = new PartyFormViewModel
        {
            Players = players.Select(player => new SelectListItem
            {
                Value = player.Id.ToString(),
                Text = player.User.UserName ?? string.Empty,
            }).ToList(),
            PlayingFields = fields.Select(field => new SelectListItem
            {
                Value = field.Id.ToString(),
                Text = field.Name,
            }).ToList(),
            Form = new PartyFormDto(),
            HmsPartyMembers = new Models.HegelMultiSelectConfig
            {
                ControlId = "hms-player-select",
                Label = "Choose players for party",
                Placeholder = "Messi",
                FieldName = "username",
                AvailableItems = players.Select(p => new Models.SelectableItem
                {
                    Id = p.Id.ToString(),
                    Name = p.User.UserName ?? string.Empty
                }).ToList()
            }
        };

        if (id.HasValue)
        {
            var partyResult = await _store.FindByIdAsync(id.Value);
            if (!partyResult.IsSuccess)
            {
                return NotFound();
            }

            var party = partyResult.Value;
            vm.Form.Id = party.Id;
            vm.Form.PlayerCreatedId = party.PlayerCreatedId;
            vm.Form.DateCreated = party.DateCreated;
            vm.Form.MaxMembers = party.MaxMembers;
            vm.Form.PartyDescription = party.PartyDescription;
            vm.Form.PreferredLocations = party.PreferredLocations;
            vm.Form.MemberIds = party.Members.Select(member => member.Id).ToList();
            vm.Form.PreferredPlayingDates = party.PreferredPlayingDates
                .OrderBy(date => date.Date)
                .Select(date => date.Date)
                .ToList();

            vm.HmsPartyMembers.SelectedItems = party.Members.Select(member => new Models.SelectableItem
            {
                Id = member.Id.ToString(),
                Name = member.User.UserName ?? string.Empty
            }).ToList();

            if (party.ScheduledMatch != null)
            {
                vm.Form.ScheduledMatchId = party.ScheduledMatch.Id;
                vm.Form.ScheduledMatchPlayingFieldId = party.ScheduledMatch.PlayingFieldId;
                vm.Form.ScheduledMatchDate = party.ScheduledMatch.MatchDate;

                var attendanceByPlayerId = party.ScheduledMatch.ScheduledMatchAttendances
                    .ToDictionary(attendance => attendance.PlayerId, attendance => attendance);

                vm.Form.ScheduledMatchAttendances = party.Members.Select(member =>
                {
                    attendanceByPlayerId.TryGetValue(member.Id, out var attendance);
                    return new PartyScheduledMatchAttendanceDto
                    {
                        Id = attendance?.Id,
                        PlayerId = member.Id,
                        PlayerName = member.User.UserName ?? string.Empty,
                        IsAttending = attendance?.IsAttending ?? false,
                    };
                }).ToList();
            }
        }

        return PartialView("_PartyForm", vm);
    }

    [HttpGet("getById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var partyResult = await _store.FindByIdAsync(id);
        if (!partyResult.IsSuccess)
            return NotFound();

        return Json(partyResult.Value);
    }


    [HttpPost("create")]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] PartyFormDto model)
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

        return Ok();
    }



    [HttpPost("edit/{id:guid}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] PartyFormDto model)
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

        return Ok();
    }


    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        var partyResult = await _store.FindByIdAsync(id);
        if (!partyResult.IsSuccess)
            return NotFound();

        await _store.DeleteByIdAsync(id);
        return Ok();
    }

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
