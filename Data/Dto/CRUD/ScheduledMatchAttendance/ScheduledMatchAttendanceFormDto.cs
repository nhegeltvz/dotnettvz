using System.ComponentModel.DataAnnotations;

namespace Data.Dto.CRUD.ScheduledMatchAttendance;

public class ScheduledMatchAttendanceFormDto
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Zakazani meč je obavezan.")]
    public Guid ScheduledMatchId { get; set; }

    [Required(ErrorMessage = "Igrač je obavezan.")]
    public Guid PlayerId { get; set; }

    public bool IsAttending { get; set; }
}
