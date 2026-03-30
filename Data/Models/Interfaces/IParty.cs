namespace Data.Models.Interfaces;

public interface IParty
{
    Guid Id { get; }
    Guid PlayerCreatedId { get; }
    DateTime DateCreated { get; }
    int MaxMembers { get; }
    string PartyDescription { get; }
    string PreferredLocations { get; }
}
