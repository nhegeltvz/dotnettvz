using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.ScheduledMatchAttendance;

public class ScheduledMatchAttendanceFormDto
{
    public Guid? Id { get; set; }

    [Required]
    public Guid ScheduledMatchId { get; set; }

    [Required]
    public Guid PlayerId { get; set; }

    public bool IsAttending { get; set; }
}
