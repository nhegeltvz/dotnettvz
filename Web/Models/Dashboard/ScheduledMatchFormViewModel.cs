using Data.Dto.CRUD.ScheduledMatch;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Models.Dashboard;

public class ScheduledMatchFormViewModel
{
    public ScheduledMatchFormDto Form { get; set; } = new();
    public List<SelectListItem> PlayingFields { get; set; } = new();
    public List<SelectListItem> Parties { get; set; } = new();
}
