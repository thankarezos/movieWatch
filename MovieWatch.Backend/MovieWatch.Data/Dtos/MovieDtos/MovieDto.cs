namespace MovieWatch.Data.Dtos.MovieDtos;

public class MovieDto
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public int? Year { get; set; }
    public double Rating { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? BannerUrl { get; set; }
    public double Popularity { get; set; }
    
}