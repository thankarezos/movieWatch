using Microsoft.Extensions.Options;
using MovieWatch.Data.Configurations;
using MovieWatch.Data.Dtos;

namespace MovieWatch.Services.Services;

public interface IMovieService
{
    Task<MoviesStringSimpleDto?> GetMovies(int page = 1, int pageSize = 20, string? titleFilter = null);
    
}

public class MovieService : IMovieService
{
    private readonly ITmdbServiceRedis _tmdbServiceRedis;
    private readonly IOptionsMonitor<TmbdConfiguration> _tmdbConfiguration;
    
    public MovieService(ITmdbServiceRedis tmdbServiceRedis, IOptionsMonitor<TmbdConfiguration> tmdbConfiguration)
    {
        _tmdbServiceRedis = tmdbServiceRedis;
        _tmdbConfiguration = tmdbConfiguration;
    }

    
    public async Task<MoviesStringSimpleDto?> GetMovies(int page = 1, int pageSize = 20, string? titleFilter = null)
    {
        var imageBaseUrl = _tmdbConfiguration.CurrentValue.ImageBaseUrl;
        return titleFilter == null ? new MoviesStringSimpleDto(await _tmdbServiceRedis.GetPaginatedMoviesFromSortedSetAsync(page, pageSize), imageBaseUrl)
            : new MoviesStringSimpleDto(await _tmdbServiceRedis.SearchMoviesByTitleAsync(titleFilter, page, pageSize), imageBaseUrl);
    }
    
}