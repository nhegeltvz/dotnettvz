using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Models.Dto;

namespace Web.Models.Dashboard;

public class PartyFormViewModel
{
    public PartyFormDto Form { get; set; } = new();
    public List<SelectListItem> Players { get; set; } = new();
    public List<SelectListItem> PlayingFields { get; set; } = new();
    public HegelMultiSelectConfig HmsPartyMembers { get; set; } = new();
    public bool HasMatchRecord { get; set; }
}
