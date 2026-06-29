using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.MatchVote;

public class MatchVoteFormDto
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Utakmica je obavezna.")]
    public Guid MatchRecordId { get; set; }

    [Required(ErrorMessage = "Igrač je obavezan.")]
    public Guid PlayerId { get; set; }

    public bool VotedHeld { get; set; }
}
