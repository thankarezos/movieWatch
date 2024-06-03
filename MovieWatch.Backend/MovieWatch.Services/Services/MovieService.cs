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
    Task<List<MovieStringSimpleDto>?> GetFavorites(int userId);
    Task AddFavorites(int userId, List<int> movieIds);
    Task RemoveFavorites(int userId, List<int> movieIds);
    
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
    
    public async Task<List<MovieStringSimpleDto>?> GetFavorites(int userId)
    {
        return await _tmdbServiceRedis.GetFavorites(userId);
    }
    
    public async Task AddFavorites(int userId, List<int> movieIds)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        
        foreach (var movieId in movieIds)
        {
            if (!await _tmdbServiceRedis.MovieExists(movieId)) continue;
            if (user?.FavoriteMovies == null) user!.FavoriteMovies = new List<int>();
            //check if movie already exists in favorites
            if (user.FavoriteMovies.Contains(movieId)) continue;
            
            user.FavoriteMovies!.Add(movieId);
            await _dbContext.SaveChangesAsync();
        }
    }
    
    public async Task RemoveFavorites(int userId, List<int> movieIds)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        
        foreach (var movieId in movieIds)
        {
            if (!await _tmdbServiceRedis.MovieExists(movieId)) continue;
            if (user?.FavoriteMovies == null) continue;
            user.FavoriteMovies!.Remove(movieId);
            await _dbContext.SaveChangesAsync();
        }
    }

}