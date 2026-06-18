using Microsoft.AspNetCore.Identity;

namespace Data.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        // Navigation to domain profile — null until the user completes their player profile
        public virtual Player? Player { get; set; }
    }
}
