using System.ComponentModel.DataAnnotations;

namespace Data.Model.DTO
{
    public class CategoryFormDto
    {
        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
    }
}
