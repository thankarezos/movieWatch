using MovieWatch.Data.Dtos;

namespace MovieWatch.Data.Response;

public class LoginResponse
{
    public required string Token { get; set; }
    public required UserDto User { get; set; }
}