using Data.Model.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Data.Model
{
    public class User : IdentityUser, IUser
    {
        [Required]
        [MaxLength(20)]
        public string DisplayName {get; set;} = string.Empty;

        //Ef navigation properties

        public virtual ICollection<Comment> Comments { get; set; } = [];
        public virtual ICollection<Ticket> AssignedTickets { get; set; } = [];
        public virtual ICollection<Ticket> ReviewingTickets { get; set; } = [];
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = [];
    }
}
