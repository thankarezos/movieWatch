namespace MovieWatch.Data.Dtos;

public class MovieSimpleCsvDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Genres { get; set; }
}