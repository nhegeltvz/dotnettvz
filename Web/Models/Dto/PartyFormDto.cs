using System.ComponentModel.DataAnnotations;

namespace Web.Models.Dto;

public class PartyFormDto
{

    public Guid? Id { get; set; }

    public Guid PlayerCreatedId { get; set; }

    public DateTime DateCreated { get; set; }

    public List<Guid> MemberIds { get; set; } = [];

    public List<DateTime> PreferredPlayingDates { get; set; } = [];

    public Guid? ScheduledMatchId { get; set; }

    public Guid? ScheduledMatchPlayingFieldId { get; set; }

    public DateTime? ScheduledMatchDate { get; set; }

    public List<PartyScheduledMatchAttendanceDto> ScheduledMatchAttendances { get; set; } = [];

    [Required]
    [Range(2, 20)]
    public int MaxMembers { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 10)]
    public string PartyDescription { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string PreferredLocations { get; set; } = string.Empty;

}
