using Microsoft.EntityFrameworkCore;
using MovieWatch.Data.Database;
using MovieWatch.Data.Models;

namespace MovieWatch.Services.Services;

public interface IWeatherService
{
    Task<IEnumerable<WeatherForecast>> GetWeatherForecast();
}

public class WeatherService : IWeatherService
{
    private MovieWatchDbContext _dbContext;
    
    public WeatherService(MovieWatchDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecast()
    {
        return await _dbContext.WeatherForecasts.ToListAsync();
    }
    
}