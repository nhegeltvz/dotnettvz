namespace MovieTheaterApi
{
    public class TMDBService
    {
        private readonly HttpClient _client;

        public TMDBService(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("TMDBApi");
        }

        // Home tabs
        public Task<string> GetNowPlayingAsync(int page = 1) =>
            GetAsync($"movie/now_playing?page={page}");

        public Task<string> GetPopularAsync(int page = 1) =>
            GetAsync($"movie/popular?page={page}");

        public Task<string> GetUpcomingAsync(int page = 1) =>
            GetAsync($"movie/upcoming?page={page}");

        public Task<string> GetTopRatedAsync(int page = 1) =>
            GetAsync($"movie/top_rated?page={page}");

        public Task<string> GetTrendingAsync(string timeWindow = "day", int page = 1) =>
            GetAsync($"trending/movie/{timeWindow}?page={page}");

        // Movie detail
        public Task<string> GetMovieAsync(int movieId) =>
            GetAsync($"movie/{movieId}");

        public Task<string> GetMovieCreditsAsync(int movieId) =>
            GetAsync($"movie/{movieId}/credits");

        public Task<string> GetMovieVideosAsync(int movieId) =>
            GetAsync($"movie/{movieId}/videos");

        public Task<string> GetMovieImagesAsync(int movieId) =>
            GetAsync($"movie/{movieId}/images");

        public Task<string> GetMovieRecommendationsAsync(int movieId, int page = 1) =>
            GetAsync($"movie/{movieId}/recommendations?page={page}");

        public Task<string> GetSimilarMoviesAsync(int movieId, int page = 1) =>
            GetAsync($"movie/{movieId}/similar?page={page}");

        public Task<string> GetMovieReviewsAsync(int movieId, int page = 1) =>
            GetAsync($"movie/{movieId}/reviews?page={page}");

        public Task<string> GetWatchProvidersAsync(int movieId) =>
            GetAsync($"movie/{movieId}/watch/providers");

        // Search & discover
        public Task<string> SearchMovieAsync(string query, int page = 1) =>
            GetAsync($"search/movie?query={Uri.EscapeDataString(query)}&page={page}");

        public Task<string> SearchMultiAsync(string query, int page = 1) =>
            GetAsync($"search/multi?query={Uri.EscapeDataString(query)}&page={page}");

        public Task<string> DiscoverMoviesAsync(string queryString) =>
            GetAsync($"discover/movie?{queryString}");

        public Task<string> GetGenreListAsync() =>
            GetAsync("genre/movie/list");

        // Person
        public Task<string> GetPersonAsync(int personId) =>
            GetAsync($"person/{personId}");

        public Task<string> GetPersonMovieCreditsAsync(int personId) =>
            GetAsync($"person/{personId}/movie_credits");

        public Task<string> GetPersonImagesAsync(int personId) =>
            GetAsync($"person/{personId}/images");

        // Config
        public Task<string> GetConfigurationAsync() =>
            GetAsync("configuration");

        // Validation
        public Task<string> ValidateKeyAsync() =>
            GetAsync("authentication");

        private async Task<string> GetAsync(string relativeUrl)
        {
            var response = await _client.GetAsync(relativeUrl);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
