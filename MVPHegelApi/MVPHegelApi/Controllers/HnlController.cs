using Microsoft.AspNetCore.Mvc;

namespace MVPHegelApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class HnlController : ControllerBase
    {
        private readonly HnlService _service;
        public HnlController(HnlService service) => _service = service;

        [HttpGet("teams")]
        public async Task<IActionResult> GetTeams()
            => Ok(await _service.GetTeamsAsync());

        [HttpGet("matches/team/{teamId}")]
        public async Task<IActionResult> GetMatchesForTeam(string teamId)
        {
            var all = await _service.GetMatchesAsync();
            var result = all
                .Where(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId)
                .OrderByDescending(m => m.Date)
                .Take(5)
                .ToList();
            return Ok(result);
        }

        [HttpGet("matches/latest")]
        public async Task<IActionResult> GetLatest()
        {
            var all = await _service.GetMatchesAsync();
            var latest = all.Where(m => m.Finished).MaxBy(m => m.Date);
            return Ok(latest);
        }
    }
}
