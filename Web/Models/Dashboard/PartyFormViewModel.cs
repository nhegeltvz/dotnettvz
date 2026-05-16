using Data.Dto.CRUD.Party;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Models.Dashboard;

public class PartyFormViewModel
{
    public PartyFormDto Form { get; set; } = new();
    public List<SelectListItem> Players { get; set; } = new();
}
