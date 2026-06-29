using Data.Dto.CRUD.PlayingField;
using Data.Dto.CRUD.User;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.api
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserApiController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        public UserApiController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
            public IActionResult Get() =>
            Ok(_userManager.Users.Select(UserDto.ToDto()));
    }
}
