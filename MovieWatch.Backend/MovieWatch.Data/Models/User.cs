using MovieWatch.Data.Common;

namespace MovieWatch.Data.Models;

public class User : IRecordable
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public List<int>? FavoriteMovies { get; set; }
    
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}