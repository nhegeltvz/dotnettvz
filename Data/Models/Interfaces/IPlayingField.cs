using Data.Data;

namespace Data.Models.Interfaces;

public interface IPlayingField
{
    Guid Id { get; }

    string Name { get; }
    string Description { get; }
    double Longitude { get; }
    double Latitude { get; }
    string ContactNumber { get; }
    FieldStatus Status { get; }
    bool IsOutdoor { get; }
    SurfaceType SurfaceType { get; }
}
