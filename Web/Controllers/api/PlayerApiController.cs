using Data.Data;
using Data.Data.Common;
using Data.Dto.CRUD.Player;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerApiController : ControllerBase
    {
        private readonly PlayerStore _store;

        public PlayerApiController(PlayerStore store)
        {
            _store = store;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> Get()
        {
            var players = await _store.QueryPlayersAsync()
                .Select(PlayerDto.ToDto())
                .ToListAsync();

            return Ok(players);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PlayerDto>> GetById(Guid id)
        {
            var result = await _store.FindByIdAsync(id);
            if (!result.IsSuccess || result.Value == null)
                return NotFound();

            return Ok(PlayerDto.ToDto().Compile()(result.Value));
        }

        // POST creates a Player profile linked to an existing AppUser (admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<PlayerDto>> Post([FromBody] PlayerFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
            player.Bio = model.Bio;
            player.PreferredPosition = (Position)model.PreferredPosition;
            player.DateOfBirth = model.DateOfBirth;

            var updateResult = await _store.UpdatePlayer(player);
            if (!updateResult.IsSuccess)
                return BadRequest(updateResult.Errors);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
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
