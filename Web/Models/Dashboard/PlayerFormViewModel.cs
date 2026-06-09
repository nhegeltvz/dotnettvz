using Data.Dto.CRUD.Player;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Models.Dashboard;

public class PlayerFormViewModel
{
    public PlayerFormDto Form { get; set; } = new();
    public List<SelectListItem> Users { get; set; } = new();
}
