using Microsoft.Extensions.Options;
using MovieWatch.Data.Configurations;
using MovieWatch.Data.Dtos;
using Newtonsoft.Json;
using RestSharp;

namespace MovieWatch.Services.Services;

public interface ITmdbService
{
    // Task<MoviesDto?> DiscoverMovies(int page = 1, bool verbose = false);
    Task<IEnumerable<MovieDto>?> DiscoverMovies(int page = 1);
    Task<List<GenreDto>?> GetGenres();
    Task<IEnumerable<string?>?> GetTrailers(int movieId);
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
                $"{baseUrl}/discover/movie?include_adult=true&include_video=false&language=en-US&page={page}&sort_by=popularity.desc");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {apiKey}");
        var response = await client.GetAsync(request);
        if (response.Content == null) return new List<MovieDto>();
        
        var genres = await GetGenres();

        var tmdbDiscoverResponse = JsonConvert.DeserializeObject<TmdbDiscoverDto>(response.Content);
        
        return tmdbDiscoverResponse?.Results?.Select(x => new MovieFullDto()
        {
            Id = x.Id,
            Title = x.Title,
            Year = x.ReleaseDate?.Year,
            GenresIds = x.GenreIds,
            Rating = Math.Round(x.VoteAverage, 1),
            Description = x.Overview,
            ImageUrl = x.PosterPath,
            BannerUrl = x.BackdropPath
        });

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
    
    public async Task<List<GenreDto>?> GetGenres()
    {
        var baseUrl = _tmdbConfiguration.CurrentValue.BaseUrl;
        var apiKey = _tmdbConfiguration.CurrentValue.ApiKey;

        var options =
            new RestClientOptions($"{baseUrl}/genre/movie/list");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {apiKey}");
        var response = await client.GetAsync(request);

        if (response.Content == null) return new List<GenreDto>();
        
        var tmdbGenresResponse = JsonConvert.DeserializeObject<TmdbGenresDto>(response.Content);
        return tmdbGenresResponse?.Genres;
    }
    
    public async Task<IEnumerable<string?>?> GetTrailers(int movieId)
    {
        var baseUrl = _tmdbConfiguration.CurrentValue.BaseUrl;
        var apiKey = _tmdbConfiguration.CurrentValue.ApiKey;

        var options =
            new RestClientOptions($"{baseUrl}/movie/{movieId}/videos");
        var client = new RestClient(options);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {apiKey}");
        var response = await client.GetAsync(request);

        if (response.Content == null) return new List<string>();
        
        var tmdbTrailersResponse = JsonConvert.DeserializeObject<TmdbTrailersDto>(response.Content);
        //get only type trailer and youtube site
        var trailerDtos = tmdbTrailersResponse?.Results?.Where(x => x.Type == "Trailer" && x.Site == "YouTube");

        return trailerDtos?.Select(x => x.Key);
    }
}