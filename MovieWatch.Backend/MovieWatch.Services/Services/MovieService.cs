using Microsoft.Extensions.Options;
using MovieWatch.Data.Common;
using MovieWatch.Data.Configurations;
using MovieWatch.Data.Database;
using MovieWatch.Data.Dtos;
using MovieWatch.Data.Dtos.MovieDtos;

namespace MovieWatch.Services.Services;

public interface IMovieService
{
    Task<MoviesStringSimpleDto?> GetMovies(int page = 1, int pageSize = 20, string? titleFilter = null);
    Task<ApiResponse<List<MovieStringSimpleDto>>?> GetFavorites(int userId);
    Task<ApiResponse> AddFavorites(int userId, List<int> movieIds);
    
}

public class MovieService : IMovieService
{
    private readonly ITmdbServiceRedis _tmdbServiceRedis;
    private readonly IOptionsMonitor<TmbdConfiguration> _tmdbConfiguration;
    private readonly MovieWatchDbContext _dbContext;
    
    public MovieService(ITmdbServiceRedis tmdbServiceRedis, 
        IOptionsMonitor<TmbdConfiguration> tmdbConfiguration,
        MovieWatchDbContext dbContext)
    {
        _tmdbServiceRedis = tmdbServiceRedis;
        _tmdbConfiguration = tmdbConfiguration;
        _dbContext = dbContext;
    }

    
    public async Task<MoviesStringSimpleDto?> GetMovies(int page = 1, int pageSize = 20, string? titleFilter = null)
    {
        var imageBaseUrl = _tmdbConfiguration.CurrentValue.ImageBaseUrl;
        return titleFilter == null ? new MoviesStringSimpleDto(await _tmdbServiceRedis.GetPaginatedMoviesFromSortedSetAsync(page, pageSize), imageBaseUrl)
            : new MoviesStringSimpleDto(await _tmdbServiceRedis.SearchMoviesByTitleAsync(titleFilter, page, pageSize), imageBaseUrl);
    }
    
    public async Task<ApiResponse<List<MovieStringSimpleDto>>?> GetFavorites(int userId)
    {
        var movies = await _tmdbServiceRedis.GetFavorites(userId);
        return new ApiResponse<List<MovieStringSimpleDto>>(movies);
    }
    
    public async Task<ApiResponse> AddFavorites(int userId, List<int> movieIds)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        
        foreach (var movieId in movieIds)
        {
            if (!await _tmdbServiceRedis.MovieExists(movieId)) continue;
            if (user?.FavoriteMovies == null) user!.FavoriteMovies = new List<int>();
            user.FavoriteMovies!.Add(movieId);
            await _dbContext.SaveChangesAsync();
        }
        
        return new ApiResponse();
    }
}