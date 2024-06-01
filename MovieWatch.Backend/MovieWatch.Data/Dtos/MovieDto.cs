namespace MovieWatch.Data.Dtos;

public class MovieDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int? Year { get; set; }
    public double Rating { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? BannerUrl { get; set; }
    public IEnumerable<string?>? TrailerUrls { get; set; }
    
}