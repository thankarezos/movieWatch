using MovieWatch.Data.Dtos.MovieDtos;

namespace MovieWatch.Data.Response;

public class ProfileResponse
{
    public required int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    
    public List<MovieStringSimpleDto>? Favorites { get; set; }
}