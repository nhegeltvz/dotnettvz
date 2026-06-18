using Data.Services;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.api
{
    [ApiController]
    [Authorize]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        private readonly AiMatchParserService _aiService;
        private readonly StadiumStore _fieldStore;

        public AiController(AiMatchParserService aiService, StadiumStore fieldStore)
        {
            _aiService = aiService;
            _fieldStore = fieldStore;
        }

        [HttpPost("parse-match")]
        public async Task<IActionResult> ParseMatch([FromBody] ParseMatchRequest request)
        {
            var fields = await _fieldStore.GetAllStadiumsAsync();
            var options = fields.Select(f => new PlayingFieldOption
            {
                Id = f.Id,
                Name = f.Name,
                Latitude = f.Latitude,
                Longitude = f.Longitude
            }).ToList();

            var result = await _aiService.ParseAsync(request.Text, options);
            return Ok(result);
        }
    }
}
public class ParseMatchRequest
{
    public string Text { get; set; } = string.Empty;
}
