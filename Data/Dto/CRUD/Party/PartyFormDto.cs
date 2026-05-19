using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.Party;

public class PartyFormDto
{
    public Guid? Id { get; set; }

    public Guid PlayerCreatedId { get; set; }

    public DateTime DateCreated { get; set; }

    public List<Guid> MemberIds { get; set; } = [];

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
