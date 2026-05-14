using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.MatchVote;

public class MatchVoteFormDto
{
    public Guid? Id { get; set; }

    [Required]
    public Guid MatchRecordId { get; set; }

    [Required]
    public Guid PlayerId { get; set; }

    public bool VotedHeld { get; set; }
}
