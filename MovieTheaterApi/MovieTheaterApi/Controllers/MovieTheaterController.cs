using Microsoft.AspNetCore.Mvc;

namespace MovieTheaterApi.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MovieTheaterController : ControllerBase
    {
        private readonly TMDBService _tmdb;

        public MovieTheaterController(TMDBService tmdb)
        {
            _tmdb = tmdb;
        }

        // ── Validation ──────────────────────────────────────────────────────────
        [HttpGet("validate")]
        public async Task<IActionResult> ValidateKey() =>
            Ok(await _tmdb.ValidateKeyAsync());

        // ── Config (cache on client) ─────────────────────────────────────────────
        [HttpGet("configuration")]
        public async Task<IActionResult> GetConfiguration() =>
            Ok(await _tmdb.GetConfigurationAsync());

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres() =>
            Ok(await _tmdb.GetGenreListAsync());

        // ── Home tabs ────────────────────────────────────────────────────────────
        [HttpGet("now-playing")]
        public async Task<IActionResult> NowPlaying([FromQuery] int page = 1) =>
            Ok(await _tmdb.GetNowPlayingAsync(page));

        [HttpGet("popular")]
        public async Task<IActionResult> Popular([FromQuery] int page = 1) =>
            Ok(await _tmdb.GetPopularAsync(page));

        [HttpGet("upcoming")]
        public async Task<IActionResult> Upcoming([FromQuery] int page = 1) =>
            Ok(await _tmdb.GetUpcomingAsync(page));

        [HttpGet("top-rated")]
        public async Task<IActionResult> TopRated([FromQuery] int page = 1) =>
            Ok(await _tmdb.GetTopRatedAsync(page));

        [HttpGet("trending/{timeWindow}")]
        public async Task<IActionResult> Trending(string timeWindow = "day", [FromQuery] int page = 1) =>
            Ok(await _tmdb.GetTrendingAsync(timeWindow, page));

        // ── Movie detail ─────────────────────────────────────────────────────────
        [HttpGet("{movieId:int}")]
        public async Task<IActionResult> GetMovie(int movieId) =>
            Ok(await _tmdb.GetMovieAsync(movieId));

        [HttpGet("{movieId:int}/credits")]
        public async Task<IActionResult> GetCredits(int movieId) =>
            Ok(await _tmdb.GetMovieCreditsAsync(movieId));

        [HttpGet("{movieId:int}/videos")]
        public async Task<IActionResult> GetVideos(int movieId) =>
            Ok(await _tmdb.GetMovieVideosAsync(movieId));

        [HttpGet("{movieId:int}/images")]
        public async Task<IActionResult> GetImages(int movieId) =>
            Ok(await _tmdb.GetMovieImagesAsync(movieId));

        [HttpGet("{movieId:int}/recommendations")]
        public async Task<IActionResult> GetRecommendations(int movieId, [FromQuery] int page = 1) =>
            Ok(await _tmdb.GetMovieRecommendationsAsync(movieId, page));

        [HttpGet("{movieId:int}/similar")]
        public async Task<IActionResult> GetSimilar(int movieId, [FromQuery] int page = 1) =>
            Ok(await _tmdb.GetSimilarMoviesAsync(movieId, page));

        [HttpGet("{movieId:int}/reviews")]
        public async Task<IActionResult> GetReviews(int movieId, [FromQuery] int page = 1) =>
            Ok(await _tmdb.GetMovieReviewsAsync(movieId, page));

        [HttpGet("{movieId:int}/watch-providers")]
        public async Task<IActionResult> GetWatchProviders(int movieId) =>
            Ok(await _tmdb.GetWatchProvidersAsync(movieId));

        // ── Search & discover ────────────────────────────────────────────────────
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int page = 1) =>
            Ok(await _tmdb.SearchMovieAsync(query, page));

        [HttpGet("search/multi")]
        public async Task<IActionResult> SearchMulti([FromQuery] string query, [FromQuery] int page = 1) =>
            Ok(await _tmdb.SearchMultiAsync(query, page));

        // Pass any TMDB discover filters as query params: genre_ids, year, vote_average.gte, sort_by, etc.
        [HttpGet("discover")]
        public async Task<IActionResult> Discover() =>
            Ok(await _tmdb.DiscoverMoviesAsync(Request.QueryString.Value?.TrimStart('?') ?? string.Empty));

        // ── Person ───────────────────────────────────────────────────────────────
        [HttpGet("person/{personId:int}")]
        public async Task<IActionResult> GetPerson(int personId) =>
            Ok(await _tmdb.GetPersonAsync(personId));

        [HttpGet("person/{personId:int}/credits")]
        public async Task<IActionResult> GetPersonCredits(int personId) =>
            Ok(await _tmdb.GetPersonMovieCreditsAsync(personId));

        [HttpGet("person/{personId:int}/images")]
        public async Task<IActionResult> GetPersonImages(int personId) =>
            Ok(await _tmdb.GetPersonImagesAsync(personId));
    }
}
