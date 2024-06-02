using Microsoft.AspNetCore.Mvc;
using MovieWatch.Data.Common;
using MovieWatch.Data.Dtos;
using MovieWatch.Data.Dtos.MovieDtos;
using MovieWatch.Data.Pld;
using MovieWatch.Services.Services;

namespace MovieWatch.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController
{
    private readonly IMovieService _movieService;
    private readonly IValidationService _validationService;
    private readonly ITmdbServiceRedis _tmdbServiceRedis;
    private readonly IPythonService _pythonService;
    
    public MoviesController(IMovieService movieService, IValidationService validationService, ITmdbServiceRedis tmdbServiceRedis, IPythonService pythonService)
    {
        _movieService = movieService;
        _validationService = validationService;
        _tmdbServiceRedis = tmdbServiceRedis;
        _pythonService = pythonService;
    }

    [HttpGet(Name = "GetMovies")]
    public async Task<ApiResponse<MoviesStringSimpleDto>?> Get([FromQuery] GetMoviesPld pld)
    {
        var validationResult = await _validationService.ValidatePldAsync<GetMoviesPld, MoviesStringSimpleDto>(pld);
        if (validationResult != null) return validationResult;
        var movies = await _movieService.GetMovies(pld.Page, pld.PageSize, pld.TitleFilter);
        return new ApiResponse<MoviesStringSimpleDto>(movies);
    }
    
    [HttpGet("GetTrailers")]
    public async Task<ApiResponse> GetTrailers()
    {
        // await _tmdbServiceRedis.GetTrailers();
        return new ApiResponse();
    }
    
    
    [HttpGet("Csv")]
    public async Task<ApiResponse> Csv()
    {
        await _tmdbServiceRedis.GenerateCsvFromRedisHash("movies.csv");
        return new ApiResponse();
    }
    
    
    [HttpGet("FetchMoviesFromTmdb")]
    public async Task<ApiResponse> FetchMovies([FromQuery] int fromPage, [FromQuery] int toPage, CancellationToken cancellationToken)
    {
        await _tmdbServiceRedis.SaveGenresToRedis();
        await _tmdbServiceRedis.AddMoviesToRedisSortedSet(fromPage, toPage, cancellationToken);
        return new ApiResponse();
    }
    
    [HttpGet("Index")]
    public async Task<ApiResponse> Index()
    {
        await _tmdbServiceRedis.IndexMoviesInRediSearch();
        return new ApiResponse();
    }
    
    [HttpGet("Python")]
    public async Task<ApiResponse> Python(CancellationToken cancellationToken)
    {
        await _tmdbServiceRedis.GenerateCsvFromRedisHash("movies.csv");
        await _pythonService.RunScript(cancellationToken);
        await _tmdbServiceRedis.SaveJsonToRedisHash();
        return new ApiResponse();
    }
    
    [HttpGet("Reco/{id}")]
    public async Task<ApiResponse<List<MovieStringSimpleDto>>> Reco(int id)
    {
        var movies = await _tmdbServiceRedis.GetRecommendations(id);
        return new ApiResponse<List<MovieStringSimpleDto>>(movies);
    }
}