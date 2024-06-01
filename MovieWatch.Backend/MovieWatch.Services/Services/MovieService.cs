using MovieWatch.Data.Dtos;

namespace MovieWatch.Services.Services;

public interface IMovieService
{
    Task<MoviesDto?> GetMovies(int page = 1, bool verbose = false);
    
}

public class MovieService : IMovieService
{
    private readonly ITmdbService _tmdbService;
    
    public MovieService(ITmdbService tmdbService)
    {
        _tmdbService = tmdbService;
    }
    
    public async Task<MoviesDto?> GetMovies(int page = 1, bool verbose = false)
    {
        //get movies from redis hash paginated
        throw new NotImplementedException();
       
    }
    
}