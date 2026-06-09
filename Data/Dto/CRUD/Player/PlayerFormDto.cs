using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.Player;

public class PlayerFormDto
{
    public Guid? Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string Bio { get; set; } = string.Empty;

    public int PreferredPosition { get; set; }

    [Required]
    public DateOnly DateOfBirth { get; set; }
}
