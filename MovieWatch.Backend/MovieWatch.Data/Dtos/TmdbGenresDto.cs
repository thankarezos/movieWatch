namespace MovieWatch.Data.Dtos;

public class TmdbGenresDto
{
    public required List<GenreDto> Genres { get; set; }
}

public class GenreDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
