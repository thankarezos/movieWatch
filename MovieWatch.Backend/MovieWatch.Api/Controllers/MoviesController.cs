using Microsoft.AspNetCore.Mvc;
using MovieWatch.Api.Filters;
using MovieWatch.Data.Common;
using MovieWatch.Data.Constants;
using MovieWatch.Data.Dtos;
using MovieWatch.Data.Dtos.MovieDtos;
using MovieWatch.Data.Models;
using MovieWatch.Data.Pld;
using MovieWatch.Services.Services;

namespace MovieWatch.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController : ControllerBase
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
    
    [Authorization(UserType.Admin, UserType.User)]
    [HttpGet("favorites", Name = "GetFavorites")]
    public async Task<ApiResponse<List<MovieStringSimpleDto>>> GetFavorites()
    {
        var user = HttpContext.Items["User"] as User;
        return new ApiResponse<List<MovieStringSimpleDto>>(await _movieService.GetFavorites(user!.Id));
    }
    
    [Authorization(UserType.Admin, UserType.User)]
    [HttpPost("favorites", Name = "AddFavorite")]
    public async Task<ApiResponse> AddFavorite([FromBody] AddFavoritePld pld)
    {
        var validationResult = await _validationService.ValidatePldAsync<AddFavoritePld, ApiResponse>(pld);
        if (validationResult != null) return validationResult;
        var user = HttpContext.Items["User"] as User;
        await _movieService.AddFavorites(user!.Id, pld.MovieIds);
        return new ApiResponse();
    }
    
    [Authorization(UserType.Admin)]
    [HttpGet("Csv")]
    public async Task<ApiResponse> Csv()
    {
        await _tmdbServiceRedis.GenerateCsvFromRedisHash("movies.csv");
        return new ApiResponse();
    }
    
    [Authorization(UserType.Admin)]
    [HttpGet("FetchMoviesFromTmdb")]
    public async Task<ApiResponse> FetchMovies([FromQuery] int fromPage, [FromQuery] int toPage, CancellationToken cancellationToken)
    {
        await _tmdbServiceRedis.SaveGenresToRedis();
        await _tmdbServiceRedis.AddMoviesToRedisSortedSet(fromPage, toPage, cancellationToken);
        return new ApiResponse();
    }
    
    [Authorization(UserType.Admin)]
    [HttpGet("Index")]
    public async Task<ApiResponse> Index()
    {
        await _tmdbServiceRedis.IndexMoviesInRediSearch();
        return new ApiResponse();
    }
    
    [Authorization(UserType.Admin)]
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
    
    [HttpGet("RecoMultiple")]
    public async Task<ApiResponse<List<MovieStringSimpleDto>>> RecoMultiple([FromQuery] string ids)
    {
        var separated = ids.Split(new char[] { ',' });
        var idsList = separated.Select(int.Parse).ToList();
        var movies = await _tmdbServiceRedis.GetRecommendations(idsList);
        return new ApiResponse<List<MovieStringSimpleDto>>(movies);
    }
}