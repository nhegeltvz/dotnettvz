using System.Linq.Expressions;
using PartyModel = Data.Models.Party;

namespace Data.Dto.CRUD.Party
{
    public class PartyDto
    {
        public Guid Id { get; set; }
        public Guid PlayerCreatedId { get; set; }
        public string PlayerCreatedUsername { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public int MaxMembers { get; set; }
        public string PartyDescription { get; set; } = string.Empty;
        public string PreferredLocations { get; set; } = string.Empty;
        public List<PreferredPlayingDateDto> PreferredPlayingDates { get; set; } = new();
        public List<PartyMemberDto> Members { get; set; } = new();
        public Guid? ScheduledMatchId { get; set; }
        public Guid? ScheduledMatchPlayingFieldId { get; set; }
        public string ScheduledMatchPlayingFieldName { get; set; } = string.Empty;
        public DateTime? ScheduledMatchDate { get; set; }
        public List<ScheduledMatchAttendanceDto> ScheduledMatchAttendances { get; set; } = new();

        public static Expression<Func<PartyModel, PartyDto>> ToDto()
        {
            return party => new PartyDto
            {
                Id = party.Id,
                PlayerCreatedId = party.PlayerCreatedId,
                PlayerCreatedUsername = party.PlayerCreated == null ? string.Empty : party.PlayerCreated.User.UserName ?? string.Empty,
                DateCreated = party.DateCreated,
                MaxMembers = party.MaxMembers,
                PartyDescription = party.PartyDescription,
                PreferredLocations = party.PreferredLocations,
                PreferredPlayingDates = party.PreferredPlayingDates == null
                    ? new List<PreferredPlayingDateDto>()
                    : party.PreferredPlayingDates
                        .Select(date => new PreferredPlayingDateDto
                        {
                            Id = date.Id,
                            Date = date.Date
                        })
                        .ToList(),
                Members = party.Members == null
                    ? new List<PartyMemberDto>()
                    : party.Members
                        .Select(member => new PartyMemberDto
                        {
                            Id = member.Id,
                            Username = member.User.UserName ?? string.Empty
                        })
                        .ToList(),
                ScheduledMatchId = party.ScheduledMatch == null ? (Guid?)null : party.ScheduledMatch.Id,
                ScheduledMatchPlayingFieldId = party.ScheduledMatch == null
                    ? (Guid?)null
                    : party.ScheduledMatch.PlayingFieldId,
                ScheduledMatchPlayingFieldName = party.ScheduledMatch == null
                    ? string.Empty
                    : party.ScheduledMatch.PlayingField == null
                        ? string.Empty
                        : party.ScheduledMatch.PlayingField.Name,
                ScheduledMatchDate = party.ScheduledMatch == null ? (DateTime?)null : party.ScheduledMatch.MatchDate,
                ScheduledMatchAttendances = party.ScheduledMatch == null
                    ? new List<ScheduledMatchAttendanceDto>()
                    : party.ScheduledMatch.ScheduledMatchAttendances == null
                        ? new List<ScheduledMatchAttendanceDto>()
                        : party.ScheduledMatch.ScheduledMatchAttendances
                            .Select(attendance => new ScheduledMatchAttendanceDto
                            {
                                PlayerId = attendance.PlayerId,
                                PlayerUsername = attendance.Player == null ? string.Empty : attendance.Player.User.UserName ?? string.Empty,
                                IsAttending = attendance.IsAttending
                            })
                            .ToList()
            };
        }
    }

    public class PreferredPlayingDateDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
    }

    public class PartyMemberDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    public class ScheduledMatchAttendanceDto
    {
        public Guid PlayerId { get; set; }
        public string PlayerUsername { get; set; } = string.Empty;
        public bool IsAttending { get; set; }
    }
}
