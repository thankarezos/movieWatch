using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MovieWatch.Services.Services;

public interface ITmdbServiceRedis
{
    Task AddMoviesToRedis(int fromPage, int toPage);
    Task SaveGenresToRedis();
}

public class TmdbServiceRedis : ITmdbServiceRedis
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ITmdbService _tmdbService;
    private readonly ILogger<TmdbServiceRedis> _logger;

    public TmdbServiceRedis(IConnectionMultiplexer connectionMultiplexer, ITmdbService tmdbService, ILogger<TmdbServiceRedis> logger)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _tmdbService = tmdbService;
        _logger = logger;
    }

    public async Task AddMoviesToRedis(int fromPage, int toPage)
    {
        var hashKey = "movies";
        var db = _connectionMultiplexer.GetDatabase();
        int delay = 1000; // Start with 1 second delay

        for (var i = fromPage; i <= toPage; i++)
        {
            try
            {
                var movies = await _tmdbService.DiscoverMovies(i);
                
                _logger.LogInformation("Adding movies from page {Page} to Redis", i);
                if (movies == null) continue;

                foreach (var movie in movies)
                {
                    var movieJson = JsonConvert.SerializeObject(movie);
                    var movieKey = movie.Id.ToString();
                    await db.HashSetAsync(hashKey, movieKey, movieJson);
                }

                // Reset delay after a successful request
                delay = 1000;

                // Introduce a delay to avoid hitting the rate limit
                await Task.Delay(100); // 100ms delay between each request
            }
            catch (Exception ex) when (ex.Message.Contains("429"))
            {
                // Handle rate limiting (429 Too Many Requests)
                _logger.LogWarning("Rate limit hit, backing off for {Delay} ms", delay);
                await Task.Delay(delay);
                delay *= 2; // Exponential backoff
                 // Retry the current page
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding movies from page {Page} to Redis", i);
                i--;
            }
        }
    }
    public async Task SaveGenresToRedis()
    {
        var hashKey = "genres";
        
        var db = _connectionMultiplexer.GetDatabase();
        var genres = await _tmdbService.GetGenres();
        if (genres == null) return;
        foreach (var genre in genres)
        {
            var genreJson = JsonConvert.SerializeObject(genre);
            var genreKey = genre.Id.ToString();
            await db.HashSetAsync(hashKey, genreKey, genreJson);
        }
    }
}