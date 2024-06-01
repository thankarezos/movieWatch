using MovieWatch.Data.Dtos;

namespace MovieWatch.Services.Services;

public interface IMovieService
{
    Task<IEnumerable<MovieDto>> GetMovies(int page = 1);
    
}

public class MovieService : IMovieService
{
    private readonly ITmdbService _tmdbService;
    
    public MovieService(ITmdbService tmdbService)
    {
        _tmdbService = tmdbService;
    }
    
    public async Task<IEnumerable<MovieDto>> GetMovies(int page = 1)
    {
        return await _tmdbService.DiscoverMovies(page);
       
    }
    
}