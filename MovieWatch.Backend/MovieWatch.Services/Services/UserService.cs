using MetroClimate.Data.Dtos.Payload;
using Microsoft.EntityFrameworkCore;
using MovieWatch.Data.Database;
using MovieWatch.Data.Models;

namespace MovieWatch.Services.Services;

public interface IUserService
{
    Task<(string? token, User? user)> Login(string username, string password);
    Task<User?> GetUserFromToken(string token);
    Task<(string? token, User? user)> Register(RegisterPld registerPld);
    
}

public class UserService : IUserService
{
    private readonly IJwtService _jwtService;
    private readonly MovieWatchDbContext _dbContext;
    
    public UserService(IJwtService jwtService, MovieWatchDbContext dbContext)
    {
        _jwtService = jwtService;
        _dbContext = dbContext;
    }
    
    public async Task<(string? token, User? user)> Login(string username, string password)
    {
        //check if user exists
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username && x.Password == password);
        
        if (user == null)
        {
            return (null, null);
        }
        
        var token = await _jwtService.GenerateJwtToken(user.Id);
        
        return (token, user);
        
    }
    
    public async Task<(string? token, User? user)> Register(RegisterPld registerPld)
    {
        var user = new User
        {
            Username = registerPld.Username,
            Password = registerPld.Password,
            Email = registerPld.Email
        };
        
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        
        var token = await _jwtService.GenerateJwtToken(user.Id);
        
        return (token, user);
    }
    
    
    public async Task<User?> GetUserFromToken(string token)
    {
        var userId = await _jwtService.GetUserIdFromToken(token);
        
        if (userId == null)
        {
            return null;
        }
        
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
    }
    
}