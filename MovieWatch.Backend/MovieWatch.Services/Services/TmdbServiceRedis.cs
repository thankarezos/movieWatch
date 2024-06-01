using Microsoft.Extensions.Logging;
using MovieWatch.Data.Dtos;
using MovieWatch.Data.Dtos.MovieDtos;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MovieWatch.Services.Services;

public interface ITmdbServiceRedis
{
    Task AddMoviesToRedisSortedSet(int fromPage, int toPage, CancellationToken cancellationToken = default);
    Task<MoviesFullDto> GetPaginatedMoviesFromSortedSetAsync(int page, int pageSize);
    Task SaveGenresToRedis();
    Task<IEnumerable<GenreDto>> GetGenresFromRedis();
    Task<(int pageCount, int totalMovies)> GetDiscoverMoviesInfo(int moviesPerPage);
    
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
    
    public async Task<IEnumerable<GenreDto>> GetGenresFromRedis()
    {
        var hashKey = "genres";
        var db = _connectionMultiplexer.GetDatabase();
        var genreJsons = await db.HashValuesAsync(hashKey);
        return genreJsons.Select(genreJson => JsonConvert.DeserializeObject<GenreDto>(genreJson!)).OfType<GenreDto>().ToList();
    }

    public async Task AddMoviesToRedisSortedSet(int fromPage, int toPage, CancellationToken cancellationToken = default)
    {
        var sortedSetKey = "movies_sorted";
        var db = _connectionMultiplexer.GetDatabase();
        int delay = 1000; // Start with 1 second delay

        for (var i = fromPage; i <= toPage; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var movies = await _tmdbService.DiscoverMovies(i);
                _logger.LogInformation("Adding movies from page {Page} to Redis sorted set", i);
                if (movies == null) continue;

                foreach (var movie in movies)
                {
                    var popularity = movie.Popularity;
                    var movieJson = JsonConvert.SerializeObject(movie);
                    await db.SortedSetAddAsync(sortedSetKey, movieJson, popularity);
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding movies from page {Page} to Redis sorted set", i);
                i--;
            }
        }
    }


    
    public async Task<MoviesFullDto> GetPaginatedMoviesFromSortedSetAsync(int page, int pageSize)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var sortedSetKey = "movies_sorted";
        var start = (page - 1) * pageSize;
        var stop = start + pageSize - 1;

        var moviesJson = await db.SortedSetRangeByRankAsync(sortedSetKey, start, stop, Order.Descending);
        
        var movieSimpleDtoList = moviesJson.Select(movieJson => JsonConvert.DeserializeObject<MovieSimpleDto>(movieJson!)).ToList();
        
        var genres = await GetGenresFromRedis();
        
        var movieFullDtoList = movieSimpleDtoList
            .Select(movieSimpleDto => new MovieFullDto(movieSimpleDto!, genres))
            .ToList();

        var moviesFullDto = new MoviesFullDto
        {
            Movies = movieFullDtoList,
            Page = page,
            TotalPages = Convert.ToInt32(Math.Ceiling(await db.SortedSetLengthAsync(sortedSetKey) / (double) pageSize)),
            TotalResults = Convert.ToInt32(await db.SortedSetLengthAsync(sortedSetKey))
        };
        
        return moviesFullDto;
    }
    
    public async Task<(int pageCount, int totalMovies)> GetDiscoverMoviesInfo(int moviesPerPage)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var sortedSetKey = "movies_sorted";
        
        var pageCount = Convert.ToInt32(Math.Ceiling(await db.SortedSetLengthAsync(sortedSetKey) / (double) moviesPerPage));
        var totalMovies = Convert.ToInt32(await db.SortedSetLengthAsync(sortedSetKey));
        
        return (pageCount, totalMovies);
    }
}