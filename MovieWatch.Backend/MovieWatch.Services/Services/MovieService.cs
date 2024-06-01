using MovieWatch.Data.Dtos;

namespace MovieWatch.Services.Services;

public interface IMovieService
{
    Task<MoviesStringSimpleDto?> GetMovies(int page = 1, int pageSize = 20);
    
}

public class MovieService : IMovieService
{
    private readonly ITmdbServiceRedis _tmdbServiceRedis;
    
    public MovieService(ITmdbServiceRedis tmdbServiceRedis)
    {
        _tmdbServiceRedis = tmdbServiceRedis;
    }
    
    public async Task<MoviesStringSimpleDto?> GetMovies(int page = 1, int pageSize = 20)
    {
       return new MoviesStringSimpleDto(await _tmdbServiceRedis.GetPaginatedMoviesFromSortedSetAsync(page, pageSize));
    }
    
}