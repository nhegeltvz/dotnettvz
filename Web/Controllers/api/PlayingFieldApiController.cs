using Data.Data;
using Data.Data.Common;
using Data.Dto.CRUD.PlayingField;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayingFieldApiController : ControllerBase
    {
        private readonly StadiumStore _store;

        public PlayingFieldApiController(StadiumStore store)
        {
            _store = store;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayingFieldDto>>> Get([FromQuery] QueryOptions<PlayingField> queryOptions)
        {
            var playingFieldsQuery = _store.QueryPlayingFieldsAsync();

            foreach (var filter in queryOptions.Filters)
            {
                playingFieldsQuery = playingFieldsQuery.Where(filter);
            }

            var playingFields = await playingFieldsQuery
                .Select(PlayingFieldDto.ToDto())
                .ToListAsync();

            return Ok(playingFields);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PlayingFieldDto>> GetById(Guid id)
        {
            var entity = await _store.FindByIdAsync(id);
            if (!entity.IsSuccess || entity.Value == null)
            {
                return NotFound();
            }

            return Ok(PlayingFieldDto.ToDto().Compile()(entity.Value));
        }

        [HttpPost]
        public async Task<ActionResult<PlayingFieldDto>> Post([FromBody] StadiumFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var playingField = new PlayingField
            {
                Id = Guid.NewGuid()
            };

            playingField.Name = model.Name;
            playingField.Description = model.Description;
            playingField.Longitude = model.Longitude ?? 0;
            playingField.Latitude = model.Latitude ?? 0;
            playingField.ContactNumber = model.ContactNumber;
            playingField.Status = (FieldStatus)model.Status;
            playingField.IsOutdoor = model.IsOutdoor;
            playingField.SurfaceType = (SurfaceType)model.SurfaceType;


            // map fields from model -> entity
            // entity.Name = model.Name; etc.

            var result = await _store.CreatePlayingField(playingField);
            if (!result.IsSuccess)
                return BadRequest(result.Errors); // or map validation errors

            // optional: load full entity for response if needed
            var createdDto = PlayingFieldDto.ToDto().Compile()(playingField);

            return CreatedAtAction(nameof(GetById), new { id = playingField.Id }, createdDto);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PlayingFieldDto>> Put(Guid id, [FromBody] StadiumFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var foundResult = await _store.FindByIdAsync(id);
            if (!foundResult.IsSuccess || foundResult.Value == null)
                return NotFound();

            var playingField = foundResult.Value;


            playingField.Name = model.Name;
            playingField.Description = model.Description;
            playingField.Longitude = model.Longitude ?? 0;
            playingField.Latitude = model.Latitude ?? 0;
            playingField.ContactNumber = model.ContactNumber;
            playingField.Status = (FieldStatus)model.Status;
            playingField.IsOutdoor = model.IsOutdoor;
            playingField.SurfaceType = (SurfaceType)model.SurfaceType;

            var updateResult = await _store.UpdatePlayingField(playingField);
            if (!updateResult.IsSuccess)
                return BadRequest(updateResult.Errors);

            return NoContent(); // or Ok(updatedDto)
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
