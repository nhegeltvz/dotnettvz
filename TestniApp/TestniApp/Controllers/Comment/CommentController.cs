using Microsoft.AspNetCore.Mvc;

namespace TestniApp.Controllers.Comment
{
    [Route("comment")]
    public class CommentController : Controller
    {
        [HttpGet("list")]
        public IActionResult List()
        {
            return View();
        }
    }
}
