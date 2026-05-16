using Data.Model.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Data.Model
{
    public class Category : ICategory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        //Ef core navigation properites

        public virtual ICollection<Ticket> Tickets { get; set; } = [];
    }
}
