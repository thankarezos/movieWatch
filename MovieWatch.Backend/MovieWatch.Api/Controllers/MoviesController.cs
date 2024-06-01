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
    
    public MoviesController(IMovieService movieService, IValidationService validationService)
    {
        _movieService = movieService;
        _validationService = validationService;
    }

    [HttpGet(Name = "GetMovies")]
    public async Task<ApiResponse<IEnumerable<MovieDto>?>> Get([FromQuery] GetMoviesPld pld)
    {
        var validationResult = await _validationService.ValidatePldAsync<GetMoviesPld, IEnumerable<MovieDto>>(pld);
        if (validationResult != null) return validationResult!;
        var movies = await _movieService.GetMovies(pld.Page);
        return new ApiResponse<IEnumerable<MovieDto>?>(movies);

    }
}