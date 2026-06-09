namespace Data.Models.Interfaces;

public interface IMatchRecord
{
    Guid Id { get; }

    bool WasMatchHeld { get; }
    DateTime MatchHeld { get; }
    Guid PlayingFieldId { get; }
    int GoalsTeamA { get; }
    int GoalsTeamB { get; }
    Guid? ScheduledMatchId { get; }
}
