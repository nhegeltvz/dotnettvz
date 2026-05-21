using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.Player;

public class PlayerFormDto
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 4)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Enter a valid email address.")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 10)]
    public string Bio { get; set; } = string.Empty;

    public int PreferredPosition { get; set; }

    [Required]
    [Range(18, 90)]
    public int? Age { get; set; }
}
