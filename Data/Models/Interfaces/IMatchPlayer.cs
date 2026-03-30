using Data.Data;

namespace Data.Models.Interfaces;

public interface IMatchPlayer
{
    Guid Id { get; }
    Guid PlayerId { get; }
    Team Team { get; }
    Guid MatchRecordId { get; }
    int Goals { get; }
    int Assists { get; }
}
