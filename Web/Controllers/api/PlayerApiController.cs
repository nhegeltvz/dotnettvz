using Data.Data;
using Data.Data.Common;
using Data.Dto.CRUD.Player;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerApiController : ControllerBase
    {
        private readonly PlayerStore _store;
        public PlayerApiController(PlayerStore store) { 
            _store = store;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> Get([FromQuery]  QueryOptions<Player> queryOptions)
        {
            var playersQuery = _store.QueryPlayersAsync();

            foreach (var filter in queryOptions.Filters)
            {
                playersQuery = playersQuery.Where(filter);
            }

            var players = await playersQuery
                .Select(PlayerDto.ToDto())
                .ToListAsync();

            return Ok(players);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PlayerDto>> GetById(Guid id)
        {
            var entity = await _store.FindByIdAsync(id);
            if (!entity.IsSuccess || entity.Value == null)
            {
                return NotFound();
            }

            return Ok(PlayerDto.ToDto().Compile()(entity.Value));
        }

        [HttpPost]
        public async Task<ActionResult<PlayerDto>> Post([FromBody] PlayerFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var player = new Player
            {
                Id = Guid.NewGuid()
            };

            player.Username = model.Username;
            player.Email = model.Email;
            player.Bio = model.Bio;
            player.PreferredPosition = (Position)model.PreferredPosition;
            player.Age = model.Age;

            var result = await _store.CreatePlayer(player);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var createdDto = PlayerDto.ToDto().Compile()(player);
            return CreatedAtAction(nameof(GetById), new { id = player.Id }, createdDto);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PlayerDto>> Put(Guid id, [FromBody] PlayerFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var foundResult = await _store.FindByIdAsync(id);
            if (!foundResult.IsSuccess || foundResult.Value == null)
                return NotFound();

            var player = foundResult.Value;
            player.Username = model.Username;
            player.Email = model.Email;
            player.Bio = model.Bio;
            player.PreferredPosition = (Position)model.PreferredPosition;
            player.Age = model.Age;

            var updateResult = await _store.UpdatePlayer(player);
            if (!updateResult.IsSuccess)
                return BadRequest(updateResult.Errors);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _store.DeleteByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
