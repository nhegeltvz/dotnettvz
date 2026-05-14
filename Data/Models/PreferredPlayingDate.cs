using Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class PreferredPlayingDate : IPreferredPlayingDate
{
    [Key]
    public Guid Id { get; set; }
    [ForeignKey(nameof(Party))]
    public Guid PartyId { get; set; }
    public DateTime Date { get; set; }

    //EF navigation properties
    public virtual Party Party { get; set; } = null!;
}
