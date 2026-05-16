using Microsoft.AspNetCore.Mvc;

namespace TestniApp.Controllers.User
{
    [Route("user")]
    public class UserController : Controller
    {
        [HttpGet("list")]
        public IActionResult List()
        {
            return View();
        }
    }
}
