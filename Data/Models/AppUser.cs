using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        [Required]
        [StringLength(11, MinimumLength = 11)]
        [RegularExpression("^[0-9]*$")]
        public string OIB { get; set; } = string.Empty;

        [Required]
        [StringLength(13, MinimumLength = 13)]
        [RegularExpression("^[0-9]*$")]
        public string JMBG { get; set; } = string.Empty;

        // Navigation to domain profile — null until the user completes their player profile
        public virtual Player? Player { get; set; }
    }
}
