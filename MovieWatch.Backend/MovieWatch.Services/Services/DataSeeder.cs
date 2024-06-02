using Microsoft.EntityFrameworkCore;
using MovieWatch.Data.Database;
using MovieWatch.Data.Models;
using Newtonsoft.Json.Linq;

namespace MovieWatch.Services.Services;
public class DataSeeder
{
    private readonly MovieWatchDbContext _dbContext;
    
    public DataSeeder(MovieWatchDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Seed<T>(JObject seedObject, string key) where T : class
    {
        var entities = ((JArray) seedObject[key]!).ToObject<IList<T>>()!;
        
        await _dbContext.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task Seed()
    {
        var seedJson = await File.ReadAllTextAsync("../seed.json");
        var seedObject = JObject.Parse(seedJson);

        if(await _dbContext.Users.AnyAsync()) return;
        
        await SeedMany<User>(seedObject, "Users");
        await _dbContext.SaveChangesAsync();
    }
    private async Task SeedMany<T>(JObject seedObject, string key) where T : class
    {
        var entities = ((JArray) seedObject[key]!).ToObject<IList<T>>()!;
        
        await _dbContext.AddRangeAsync(entities);
    }
}
