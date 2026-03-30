namespace Data.Models.Interfaces;

public interface IMatchVote
{
    Guid Id { get; }
    Guid MatchRecordId { get; }
    Guid PlayerId { get; }
    bool VotedHeld { get; }
}
