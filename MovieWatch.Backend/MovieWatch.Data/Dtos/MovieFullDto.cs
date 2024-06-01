namespace MovieWatch.Data.Dtos;

public class MovieFullDto : MovieDto
{
    public List<int>? GenresIds { get; set; }
}