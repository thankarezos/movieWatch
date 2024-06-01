using Microsoft.Extensions.Options;
using MovieWatch.Data.Configurations;
using MovieWatch.Data.Dtos;
using Newtonsoft.Json;
using RestSharp;

namespace MovieWatch.Services.Services;

public interface ITmdbService
{
    Task<IEnumerable<MovieDto>?> DiscoverMovies(int page = 1);
    Task<(int pageCount, int totalMovies, int moviesPerPage)> GetDiscoverMoviesInfo();
    
}

public class TmdbService : ITmdbService
{
    private readonly IOptionsMonitor<TmbdConfiguration> _tmdbConfiguration;
    
    public TmdbService(IOptionsMonitor<TmbdConfiguration> tmdbConfiguration)
    {
        _tmdbConfiguration = tmdbConfiguration;
    }

    public async Task<IEnumerable<MovieDto>?> DiscoverMovies(int page = 1)
    {
        var baseUrl = _tmdbConfiguration.CurrentValue.BaseUrl;
        var apiKey = _tmdbConfiguration.CurrentValue.ApiKey;

        var options =
            new RestClientOptions(
                $"{baseUrl}/discover/movie?include_adult=true&include_video=true&language=en-US&page={page}&sort_by=popularity.desc");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {apiKey}");
        var response = await client.GetAsync(request);


        if (response.Content == null) return new List<MovieDto>();
        
        var tmdbDiscoverResponse = JsonConvert.DeserializeObject<TmdbDiscoverDto>(response.Content);
        var movies = tmdbDiscoverResponse?.Results?.Select(x => new MovieDto
        {
            Id = x.Id,
            Title = x.Title,
            Year = x.ReleaseDate.Year,
        });
        return movies;

    }
    
    public async Task<(int pageCount, int totalMovies, int moviesPerPage)> GetDiscoverMoviesInfo()
    {
        var baseUrl = _tmdbConfiguration.CurrentValue.BaseUrl;
        var apiKey = _tmdbConfiguration.CurrentValue.ApiKey;

        var options =
            new RestClientOptions(
                $"{baseUrl}/discover/movie?include_adult=true&include_video=true&language=en-US&page=1&sort_by=popularity.desc");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {apiKey}");
        var response = await client.GetAsync(request);

        if (response.Content == null) return (0, 0, 0);
        
        var tmdbDiscoverResponse = JsonConvert.DeserializeObject<TmdbDiscoverDto>(response.Content);
        return (tmdbDiscoverResponse!.TotalPages, tmdbDiscoverResponse.TotalResults, tmdbDiscoverResponse.Results!.Count);
        
    }
}