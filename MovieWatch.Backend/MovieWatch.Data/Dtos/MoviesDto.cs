using MovieWatch.Data.Dtos.MovieDtos;

namespace MovieWatch.Data.Dtos;

public class MoviesDto
{
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int TotalResults { get; set; }
}