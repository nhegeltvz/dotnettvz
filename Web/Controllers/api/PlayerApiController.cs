using Data.Data;
using Data.Data.Common;
using Data.Dto.CRUD.Player;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers.api
{
    [Route("api/players")]
    [ApiController]
    public class PlayerApiController : ControllerBase
    {
        private readonly PlayerStore _store;
        private readonly UserManager<AppUser> _userManager;

        public PlayerApiController(PlayerStore store, UserManager<AppUser> userManager)
        {
            _store = store;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> Get(string? search)
        {
            var playersQuery = _store.QueryPlayersAsync();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                var term = search.ToLower();
                playersQuery = playersQuery.Where(p => p.User.UserName.ToLower().Contains(term));
            }

            var players = await playersQuery
                .Select(PlayerDto.ToDto())
                .ToListAsync();

            return Ok(players);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PlayerDto>> GetById(Guid id)
        {
            var result = await _store.FindByIdAsync(id);
            if (!result.IsSuccess || result.Value == null)
                return NotFound();

            return Ok(PlayerDto.ToDto().Compile()(result.Value));
        }

        // POST creates a Player profile linked to an existing AppUser (admin only)
        [Authorize(Roles = AppRoles.ADMIN_ROLE)]
        [HttpPost]
        public async Task<ActionResult<PlayerDto>> Post([FromBody] PlayerFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                return BadRequest("User not found.");

            var existingPlayer = await _store.FindByUserIdAsync(model.UserId);
            if (existingPlayer.IsSuccess)
                return BadRequest("This user already has a player profile.");

            var player = new Player
            {
                Id = Guid.NewGuid(),
                UserId = model.UserId,
                Bio = model.Bio,
                PreferredPosition = (Position)model.PreferredPosition,
                DateOfBirth = model.DateOfBirth,
            };

            var result = await _store.CreatePlayer(player);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var created = await _store.FindByIdAsync(player.Id);
            if (!created.IsSuccess || created.Value == null)
                return StatusCode(500);

            return CreatedAtAction(nameof(GetById), new { id = player.Id },
                PlayerDto.ToDto().Compile()(created.Value));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] PlayerFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var foundResult = await _store.FindByIdAsync(id);
            if (!foundResult.IsSuccess || foundResult.Value == null)
                return NotFound();

            var player = foundResult.Value;
            player.UserId = model.UserId;
            player.Bio = model.Bio;
            player.PreferredPosition = (Position)model.PreferredPosition;
            player.DateOfBirth = model.DateOfBirth;

            var updateResult = await _store.UpdatePlayer(player);
            if (!updateResult.IsSuccess)
                return BadRequest(updateResult.Errors);

            return NoContent();
        }

        [Authorize(Roles = AppRoles.ADMIN_ROLE)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _store.DeleteByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound();

            return NoContent();
        }
    }
}
