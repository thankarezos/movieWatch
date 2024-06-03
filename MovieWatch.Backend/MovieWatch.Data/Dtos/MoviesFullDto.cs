using MovieWatch.Data.Dtos.MovieDtos;

namespace MovieWatch.Data.Dtos;

public class MoviesFullDto : MoviesDto
{
    public IEnumerable<MovieFullDto>? Movies { get; set; }
}