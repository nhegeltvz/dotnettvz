namespace Web.Models;

public class PlayerChipViewModel
{
    public Guid PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public double? AverageRating { get; set; }

    public string RatingColorClass => AverageRating switch
    {
        null => "chip-rating--none",
        < 6 => "chip-rating--red",
        < 7 => "chip-rating--yellow",
        < 8 => "chip-rating--green",
        _ => "chip-rating--blue"
    };

    public string RatingDisplay => AverageRating.HasValue
        ? AverageRating.Value.ToString("0.0")
        : "—";

    public string Initials => string.IsNullOrWhiteSpace(Username)
        ? "P"
        : string.Join(string.Empty,
            Username.Split(['_', '-', '.'], StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => char.ToUpperInvariant(part[0])));
}
