namespace MovieWatch.Data.Dtos.MovieDtos;

public class MovieSimpleDto : MovieDto
{
    public List<int>? GenresIds { get; set; }
    
}