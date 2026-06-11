using Data.Data.Common;
using Data.Dto.CRUD.MatchRecord;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchRecordApiController : ControllerBase
    {
        private readonly MatchStore _matchStore;
        public MatchRecordApiController(MatchStore store)
        {
            _matchStore = store;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchRecordDto>>> Get([FromQuery] QueryOptions<MatchRecord> queryOptions)
        {
            var matchesQuery = _matchStore.QueryMatchesAsync();

            foreach (var filter in queryOptions.Filters)
            {
                matchesQuery = matchesQuery.Where(filter);
            }

            var matches = await matchesQuery
                .Select(MatchRecordDto.ToDto())
                .ToListAsync();

            return Ok(matches);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MatchRecordDto>> GetById(Guid id)
        {
            var entity = await _matchStore.FindByIdAsync(id);
            if (!entity.IsSuccess || entity.Value == null)
            {
                return NotFound();
            }
            try
            {
                var dto = MatchRecordDto.ToDto().Compile()(entity.Value);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + " | " + ex.InnerException?.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<MatchRecordDto>> Post([FromBody] MatchRecordFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var matchRecord = new MatchRecord
            {
                Id = Guid.NewGuid()
            };

            matchRecord.WasMatchHeld = model.WasMatchHeld;
            matchRecord.MatchHeld = model.MatchHeld;
            matchRecord.PlayingFieldId = model.PlayingFieldId;
            matchRecord.GoalsTeamA = model.GoalsTeamA;
            matchRecord.GoalsTeamB = model.GoalsTeamB;

            var result = await _matchStore.CreateMatchRecord(matchRecord);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var createdDto = MatchRecordDto.ToDto().Compile()(matchRecord);
            return CreatedAtAction(nameof(GetById), new { id = matchRecord.Id }, createdDto);
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MatchRecordDto>> Put(Guid id, [FromBody] MatchRecordFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var foundResult = await _matchStore.FindByIdAsync(id);
            if (!foundResult.IsSuccess || foundResult.Value == null)
                return NotFound();

            var matchRecord = foundResult.Value;
            matchRecord.WasMatchHeld = model.WasMatchHeld;
            matchRecord.MatchHeld = model.MatchHeld;
            matchRecord.PlayingFieldId = model.PlayingFieldId;
            matchRecord.GoalsTeamA = model.GoalsTeamA;
            matchRecord.GoalsTeamB = model.GoalsTeamB;

            var updateResult = await _matchStore.UpdateMatchRecord(matchRecord);
            if (!updateResult.IsSuccess)
                return BadRequest(updateResult.Errors);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _matchStore.DeleteByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
