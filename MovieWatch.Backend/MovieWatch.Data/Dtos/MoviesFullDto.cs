namespace MovieWatch.Data.Dtos;

public class MoviesFullDto : MoviesDto
{
    public IEnumerable<MovieFullDto>? Movies { get; set; }
    
    public int TotalPages { get; set; }
    public int TotalResults { get; set; }
    public int MoviesPerPage { get; set; }
}