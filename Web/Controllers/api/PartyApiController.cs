using Data.Data.Common;
using Data.Dto.CRUD.Party;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartyApiController : ControllerBase
    {
        private readonly PartyStore _store;

        public PartyApiController(PartyStore store)
        {
            _store = store;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartyDto>>> Get([FromQuery] QueryOptions<Party> queryOptions)
        {
            var partiesQuery = _store.QueryPartiesAsync();

            foreach (var filter in queryOptions.Filters)
            {
                partiesQuery = partiesQuery.Where(filter);
            }

            var parties = await partiesQuery
                .Select(PartyDto.ToDto())
                .ToListAsync();

            return Ok(parties);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PartyDto>> GetById(Guid id)
        {
            var entity = await _store.FindByIdAsync(id);
            if (!entity.IsSuccess || entity.Value == null)
            {
                return NotFound();
            }

            return Ok(PartyDto.ToDto().Compile()(entity.Value));
        }

        [HttpPost]
        public async Task<ActionResult<PartyDto>> Post([FromBody] PartyListDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var party = new Party
            {
                Id = Guid.NewGuid()
            };

            party.PlayerCreatedId = model.PlayerCreatedId;
            party.DateCreated = model.DateCreated;
            party.MaxMembers = model.MaxMembers;
            party.PartyDescription = model.PartyDescription;
            party.PreferredLocations = model.PreferredLocations;

            var result = await _store.CreateParty(party);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var createdDto = PartyDto.ToDto().Compile()(party);
            return CreatedAtAction(nameof(GetById), new { id = party.Id }, createdDto);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PartyDto>> Put(Guid id, [FromBody] PartyListDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var foundResult = await _store.FindByIdAsync(id);
            if (!foundResult.IsSuccess || foundResult.Value == null)
                return NotFound();

            var party = foundResult.Value;
            party.PlayerCreatedId = model.PlayerCreatedId;
            party.DateCreated = model.DateCreated;
            party.MaxMembers = model.MaxMembers;
            party.PartyDescription = model.PartyDescription;
            party.PreferredLocations = model.PreferredLocations;

            var updateResult = await _store.UpdateParty(party);
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
