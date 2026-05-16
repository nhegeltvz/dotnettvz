using Microsoft.AspNetCore.Mvc;

namespace TestniApp.Views.Ticket
{
    [Route("ticket")]
    public class TicketController : Controller
    {
        [HttpGet("list")]
        public IActionResult List()
        {
            return View();
        }
    }


}
