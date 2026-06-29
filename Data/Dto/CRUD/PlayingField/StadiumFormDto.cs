using Data.Data;
using Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.PlayingField;

public class StadiumFormDto
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Naziv mora imati između 3 i 50 znakova.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Opis ne smije biti duži od 500 znakova.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Geografska dužina je obavezna.")]
    [Range(-180.0, 180.0, ErrorMessage = "Geografska dužina mora biti između -180 i 180.")]
    public double? Longitude { get; set; }

    [Required(ErrorMessage = "Geografska širina je obavezna.")]
    [Range(-90.0, 90.0, ErrorMessage = "Geografska širina mora biti između -90 i 90.")]
    public double? Latitude { get; set; }

    [StringLength(40, ErrorMessage = "Kontakt broj ne smije biti duži od 40 znakova.")]
    public string ContactNumber { get; set; } = string.Empty;

    public int Status { get; set; }

    public bool IsOutdoor { get; set; }

    public int SurfaceType { get; set; }

    public List<Guid> ImageIds { get; set; } = new();

    public override string ToString() =>
        $"Name={Name}, Surface={SurfaceType}, Outdoor={IsOutdoor}, Status={Status}, Lat={Latitude}, Lon={Longitude}, Contact={ContactNumber}";
}
