using Data.Data;

namespace Data.Models.Interfaces;

public interface IPlayer
{
    Guid Id { get; }
    string Bio { get; }
    Position PreferredPosition { get; }
    DateOnly DateOfBirth { get; }
}
