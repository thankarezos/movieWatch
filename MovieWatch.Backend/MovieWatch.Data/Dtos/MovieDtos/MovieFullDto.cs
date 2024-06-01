using System.Diagnostics.CodeAnalysis;

namespace MovieWatch.Data.Dtos.MovieDtos;

public class MovieFullDto : MovieDto
{
    public List<GenreDto>? Genres { get; set; }
    
    [SetsRequiredMembers]
    public MovieFullDto(MovieSimpleDto movieSimpleDto, IEnumerable<GenreDto> genres)
    {
        Id = movieSimpleDto.Id;
        Title = movieSimpleDto.Title;
        Description = movieSimpleDto.Description;
        Year = movieSimpleDto.Year;
        Rating = movieSimpleDto.Rating;
        ImageUrl = movieSimpleDto.ImageUrl;
        BannerUrl = movieSimpleDto.BannerUrl;
        Popularity = movieSimpleDto.Popularity;
        Genres = genres.Where(g => movieSimpleDto.GenresIds?.Contains(g.Id) == true).ToList();
    }
}