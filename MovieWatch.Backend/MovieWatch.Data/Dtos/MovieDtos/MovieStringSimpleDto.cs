using System.Diagnostics.CodeAnalysis;

namespace MovieWatch.Data.Dtos.MovieDtos;

public class MovieStringSimpleDto : MovieDto
{
    public List<string>? Genres { get; set; }
    
    [SetsRequiredMembers]
    public MovieStringSimpleDto(MovieFullDto movie)
    {
        Id = movie.Id;
        Title = movie.Title;
        Description = movie.Description;
        Year = movie.Year;
        Rating = movie.Rating;
        ImageUrl = movie.ImageUrl;
        BannerUrl = movie.BannerUrl;
        Popularity = movie.Popularity;
        Genres = movie.Genres?.Select(g => g.Name).ToList();
    }
    
    
}