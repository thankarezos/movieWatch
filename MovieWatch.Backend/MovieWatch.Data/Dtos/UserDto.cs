using MovieWatch.Data.Models;

namespace MovieWatch.Data.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    
    public UserDto(User user)
    {
        Id = user.Id;
        Username = user.Username;
        Email = user.Email;
    }
}