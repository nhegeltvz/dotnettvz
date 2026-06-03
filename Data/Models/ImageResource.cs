using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public class ImageResource
{
    [Key]
    public Guid Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }

    public Guid? PlayingFieldId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual PlayingField? PlayingField { get; set; }
}
