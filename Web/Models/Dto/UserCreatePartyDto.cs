using System.ComponentModel.DataAnnotations;

namespace Web.Models.Dto;

public class UserCreatePartyDto
{
    [Required]
    [Range(2, 20)]
    public int MaxMembers { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 10)]
    public string PartyDescription { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string PreferredLocations { get; set; } = string.Empty;

    public List<DateTime> PreferredPlayingDates { get; set; } = [];
}
