using Data.Dto.CRUD.MatchRecord;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Models;

namespace Web.Models.Dashboard;

public class MatchRecordFormViewModel
{
    public MatchRecordFormDto Form { get; set; } = new();
    public List<SelectListItem> Stadiums { get; set; } = new();
    public HegelMultiSelectConfig HmsMatchPlayers { get; set; } = new();
}
