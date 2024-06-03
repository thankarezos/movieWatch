using MovieWatch.Data.Dtos.MovieDtos;
using System.Diagnostics.CodeAnalysis;

namespace MovieWatch.Data.Dtos;

public class MoviesStringSimpleDto : MoviesDto
{
    public List<MovieStringSimpleDto>? Movies { get; set; }
    
    
    public MoviesStringSimpleDto(MoviesFullDto moviesFullDto, string imageBaseUrl)
    {
        Movies = moviesFullDto.Movies?.Select(m => new MovieStringSimpleDto(m, imageBaseUrl)).ToList();
        TotalPages = moviesFullDto.TotalPages;
        TotalResults = moviesFullDto.TotalResults;
        Page = moviesFullDto.Page;
    }
}