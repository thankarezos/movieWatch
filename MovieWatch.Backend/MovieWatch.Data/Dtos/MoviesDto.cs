namespace MovieWatch.Data.Dtos;

public class MoviesDto
{
    public IEnumerable<MovieDto>? Movies { get; set; }
    
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int TotalResults { get; set; }
    public int MoviesPerPage { get; set; }
}