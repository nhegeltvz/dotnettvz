using Data.Data;

namespace Data.Models.Interfaces;

public interface IPlayer
{
    Guid Id { get; }
    string Username { get; }
    string Email { get; }
    string Bio { get; }
    Position PreferredPosition { get; }
    int? Age { get; }
    byte[] ProfilePicture { set; } 
}
