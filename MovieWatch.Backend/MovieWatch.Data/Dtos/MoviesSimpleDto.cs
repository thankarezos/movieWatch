using MovieWatch.Data.Dtos.MovieDtos;
using System.Diagnostics.CodeAnalysis;

namespace MovieWatch.Data.Dtos;

public class MoviesStringSimpleDto : MoviesDto
{
    public List<MovieStringSimpleDto>? Movies { get; set; }
    
    
    public MoviesStringSimpleDto(MoviesFullDto moviesFullDto)
    {
        Movies = moviesFullDto.Movies?.Select(movie => new MovieStringSimpleDto(movie)).ToList();
    }
}