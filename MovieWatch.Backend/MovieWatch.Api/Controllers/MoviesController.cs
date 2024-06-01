using Microsoft.AspNetCore.Mvc;
using MovieWatch.Data.Common;
using MovieWatch.Data.Dtos;
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
    
    public MoviesController(IMovieService movieService, IValidationService validationService, ITmdbServiceRedis tmdbServiceRedis)
    {
        _movieService = movieService;
        _validationService = validationService;
        _tmdbServiceRedis = tmdbServiceRedis;
    }

    [HttpGet(Name = "GetMovies")]
    public async Task<ApiResponse<MoviesStringSimpleDto>?> Get([FromQuery] GetMoviesPld pld)
    {
        var validationResult = await _validationService.ValidatePldAsync<GetMoviesPld, MoviesStringSimpleDto>(pld);
        if (validationResult != null) return validationResult;
        var movies = await _movieService.GetMovies(pld.Page, pld.PageSize);
        return new ApiResponse<MoviesStringSimpleDto>(movies);

    }
    
    [HttpGet("Index")]
    public async Task<ApiResponse<MoviesDto>?> Index([FromQuery] int fromPage, [FromQuery] int toPage, CancellationToken cancellationToken)
    {
        await _tmdbServiceRedis.SaveGenresToRedis();
        await _tmdbServiceRedis.AddMoviesToRedisSortedSet(fromPage, toPage, cancellationToken);
        return new ApiResponse<MoviesDto>();
    }
}