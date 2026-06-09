using System.ComponentModel.DataAnnotations;

namespace Web.Models.Dto;

public class ScheduleMatchDto
{
    [Required]
    public Guid PlayingFieldId { get; set; }

    [Required]
    public DateTime MatchDate { get; set; }
}
