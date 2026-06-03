using Data.Data;
using Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public class PlayingField : IPlayingField
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public byte[]? Image { get; set; }
    public string ContactNumber { get; set; } = string.Empty;
    public FieldStatus Status { get; set; }
    public bool IsOutdoor { get; set; }
    public SurfaceType SurfaceType { get; set; }

    //EF navigation properties
    public virtual ICollection<MatchRecord> MatchRecords { get; set; } = new List<MatchRecord>();
    public virtual ICollection<ScheduledMatch> ScheduledMatches { get; set; } = new List<ScheduledMatch>();
    public virtual ICollection<ImageResource> Images { get; set; } = new List<ImageResource>();
}
