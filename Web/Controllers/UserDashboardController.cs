using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Authorize]
[Route("user-dashboard")]
public class UserDashboardController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
}
