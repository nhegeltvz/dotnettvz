using Data.Dto.CRUD.MatchRecord;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Models.Dashboard;

public class MatchRecordFormViewModel
{
    public MatchRecordFormDto Form { get; set; } = new();
    public List<SelectListItem> Stadiums { get; set; } = new();
}
