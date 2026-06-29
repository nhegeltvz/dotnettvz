using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.Player;

public class PlayerFormDto
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Korisnik je obavezan.")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Biografija je obavezna.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Biografija mora imati između 10 i 500 znakova.")]
    public string Bio { get; set; } = string.Empty;

    public int PreferredPosition { get; set; }

    [Required(ErrorMessage = "Datum rođenja je obavezan.")]
    public DateOnly DateOfBirth { get; set; }

    public override string ToString() =>
        $"Position={PreferredPosition}, DOB={DateOfBirth}, Bio={Bio}";
}
