using Data.Data;
using Data.Models.Interfaces;

namespace Data.Models;

public class MatchPlayer : IMatchPlayer
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public Team Team { get; set; }
    public Guid MatchRecordId { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }

    //EF navigation properties
    public virtual Player Player { get; set; } = null!;
    public virtual MatchRecord MatchRecord { get; set; } = null!;
    public virtual PlayerRating? PlayerRating { get; set; }
}
