using Data.Models;

namespace Web.Models;

public class StadiumDetailsViewModel
{
    public PlayingField Field { get; set; } = null!;
    public int PlayedMatchesCount { get; set; }
}
