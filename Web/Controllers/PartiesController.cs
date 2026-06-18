using Data.Data;
using Data.Dto.CRUD.Party;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles = AppRoles.ADMIN_ROLE)]
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
}
