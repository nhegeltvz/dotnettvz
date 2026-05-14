using Data.Data;
using Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.PlayingField;

public class StadiumFormDto
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(-180.0, 180.0)]
    public double Longitude { get; set; }

    [Required]
    [Range(-90.0, 90.0)]
    public double Latitude { get; set; }

    [StringLength(40)]
    public string ContactNumber { get; set; } = string.Empty;

    public FieldStatus Status { get; set; }

    public bool IsOutdoor { get; set; }

    public SurfaceType SurfaceType { get; set; }
}
