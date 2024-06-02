using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieWatch.Data.Configurations;
using MovieWatch.Data.Dtos;
using MovieWatch.Data.Dtos.MovieDtos;
using Newtonsoft.Json;
using StackExchange.Redis;
using NRediSearch;

namespace MovieWatch.Services.Services;

public interface ITmdbServiceRedis
{
    Task AddMoviesToRedisSortedSet(int fromPage, int toPage, CancellationToken cancellationToken = default);
    Task<MoviesFullDto> GetPaginatedMoviesFromSortedSetAsync(int page, int pageSize);
    Task SaveGenresToRedis();
    Task IndexMoviesInRediSearch();
    Task<MoviesFullDto> SearchMoviesByTitleAsync(string titleFilter, int page, int pageSize);
    Task GenerateCsvFromRedisHash(string fileName);
    Task SaveJsonToRedisHash();
    Task<List<MovieStringSimpleDto>> GetRecommendations(int id);
}

public class TmdbServiceRedis : ITmdbServiceRedis
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ITmdbService _tmdbService;
    private readonly IOptionsMonitor<PyhtonConfiguration> _pyhtonConfiguration;
    private readonly IOptionsMonitor<TmbdConfiguration> _tmdbConfiguration;
    private readonly ILogger<TmdbServiceRedis> _logger;

    public TmdbServiceRedis(IConnectionMultiplexer connectionMultiplexer, ITmdbService tmdbService, ILogger<TmdbServiceRedis> logger, IOptionsMonitor<PyhtonConfiguration> pyhtonConfiguration, IOptionsMonitor<TmbdConfiguration> tmdbConfiguration)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _tmdbService = tmdbService;
        _logger = logger;
        _pyhtonConfiguration = pyhtonConfiguration;
        _tmdbConfiguration = tmdbConfiguration;
    }
    
    public async Task SaveGenresToRedis()
    {
        var hashKey = "genres";
        
        var db = _connectionMultiplexer.GetDatabase();
        var genres = await _tmdbService.GetGenres();
        if (genres == null) return;
        foreach (var genre in genres)
        {
            var genreJson = JsonConvert.SerializeObject(genre);
            var genreKey = genre.Id.ToString();
            await db.HashSetAsync(hashKey, genreKey, genreJson);
        }
    }
    
    public async Task<IEnumerable<GenreDto>> GetGenresFromRedis()
    {
        var hashKey = "genres";
        var db = _connectionMultiplexer.GetDatabase();
        var genreJsons = await db.HashValuesAsync(hashKey);
        return genreJsons.Select(genreJson => JsonConvert.DeserializeObject<GenreDto>(genreJson!)).OfType<GenreDto>().ToList();
    }

    public async Task AddMoviesToRedisSortedSet(int fromPage, int toPage, CancellationToken cancellationToken = default)
    {
        var sortedSetKey = "movies_sorted";
        var hashKey = "movies_hash";
        var db = _connectionMultiplexer.GetDatabase();
        int delay = 1000; // Start with 1 second delay

        for (var i = fromPage; i <= toPage; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var movies = await _tmdbService.DiscoverMovies(i);
                _logger.LogInformation("Adding movies from page {Page} to Redis sorted set", i);
                if (movies == null) continue;

                foreach (var movie in movies)
                {
                    var movieId = movie.Id.ToString();
                    var popularity = movie.Popularity;
                    var movieJson = JsonConvert.SerializeObject(movie);
                    await db.HashSetAsync(hashKey, movieId, movieJson);
                    await db.SortedSetAddAsync(sortedSetKey, movieId, popularity);
                }

                // Reset delay after a successful request
                delay = 1000;

                // Introduce a delay to avoid hitting the rate limit
                await Task.Delay(100, cancellationToken); // 100ms delay between each request
            }
            catch (Exception ex) when (ex.Message.Contains("429"))
            {
                // Handle rate limiting (429 Too Many Requests)
                _logger.LogWarning("Rate limit hit, backing off for {Delay} ms", delay);
                await Task.Delay(delay, cancellationToken);
                delay *= 2; // Exponential backoff
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding movies from page {Page} to Redis sorted set", i);
                i--;
            }
        }
    }


    
    public async Task<MoviesFullDto> GetPaginatedMoviesFromSortedSetAsync(int page, int pageSize)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var sortedSetKey = "movies_sorted";
        var start = (page - 1) * pageSize;
        var stop = start + pageSize - 1;

        var moviesJsonIds = await db.SortedSetRangeByRankAsync(sortedSetKey, start, stop, Order.Descending);
        //get movie jsons from hash
        var moviesJson = await db.HashGetAsync("movies_hash", moviesJsonIds.Select(x => x).ToArray());
        
        var movieSimpleDtoList = moviesJson.Select(movieJson => JsonConvert.DeserializeObject<MovieSimpleDto>(movieJson!)).ToList();
        
        var genres = await GetGenresFromRedis();
        
        var movieFullDtoList = movieSimpleDtoList
            .Select(movieSimpleDto => new MovieFullDto(movieSimpleDto!, genres))
            .ToList();

        var moviesFullDto = new MoviesFullDto
        {
            Movies = movieFullDtoList,
            Page = page,
            TotalPages = Convert.ToInt32(Math.Ceiling(await db.SortedSetLengthAsync(sortedSetKey) / (double) pageSize)),
            TotalResults = Convert.ToInt32(await db.SortedSetLengthAsync(sortedSetKey))
        };
        
        return moviesFullDto;
    }

    public async Task<MoviesFullDto> SearchMoviesByTitleAsync(string titleFilter, int page, int pageSize)
    {
        titleFilter = titleFilter.ToLower().Trim();
        
        var db = _connectionMultiplexer.GetDatabase();
        var indexName = "movies_index";
        var client = new Client(indexName, db);

        // Build the search query
        var query = new Query($"@title:{titleFilter}*")
            .Limit((page - 1) * pageSize, pageSize);

        // Execute the search query
        var result = await client.SearchAsync(query);

        // Get the movie IDs from the search result
        var movieIds = result.Documents.Select(document => document.Id.Replace("movie:", "")).ToList();

        // Fetch movie data from the sorted set using the IDs
        var moviesJson = new List<RedisValue>();

        foreach (var movieId in movieIds)
        {
            var movieJson = await db.HashGetAsync("movies_hash", movieId);
            if (movieJson.HasValue)
            {
                moviesJson.Add(movieJson);
            }
        }

        // Deserialize movies
        var movieSimpleDtoList = moviesJson
            .Select(movieJson => JsonConvert.DeserializeObject<MovieSimpleDto>(movieJson!))
            .Where(movie => movie != null)
            .ToList();

        // Fetch genres from Redis
        var genres = await GetGenresFromRedis();

        // Convert to MovieFullDto
        var movieFullDtoList = movieSimpleDtoList
            .Select(movieSimpleDto => new MovieFullDto(movieSimpleDto!, genres))
            .ToList();

        // Create MoviesFullDto
        var moviesFullDto = new MoviesFullDto
        {
            Movies = movieFullDtoList,
            Page = page,
            TotalPages = Convert.ToInt32(Math.Ceiling(result.TotalResults / (double)pageSize)),
            TotalResults = Convert.ToInt32(result.TotalResults)
        };

        return moviesFullDto;
    }
    
    
    public async Task IndexMoviesInRediSearch()
    {
        var db = _connectionMultiplexer.GetDatabase();
        var indexName = "movies_index";
        await db.ExecuteAsync("FT.CONFIG", "SET", "MINPREFIX", "1");
        
        var moviesJson = await db.HashGetAsync("movies_hash", db.HashKeys("movies_hash"));
        
        var movies = moviesJson.Select(movieJson => JsonConvert.DeserializeObject<MovieSimpleDto>(movieJson!)).ToList();
        
        var client = new Client(indexName, db);
        
        var schema = new Schema()
            .AddTextField("title", 1.0)
            .AddSortableNumericField("popularity")
            .AddTagField("genres");

        var options = new Client.ConfiguredIndexOptions()
            .SetNoStopwords();
        
        //if index exists do alter instead of create
        try
        {
            await client.CreateIndexAsync(schema, options);
        }
        catch (Exception e)
        {
            if (!e.Message.Contains("Index already exists")) 
                _logger.LogError(e, "Skipping index creation");
        }
        
        
        foreach (var movie in movies.OfType<MovieSimpleDto>())
        {
            //if document already exists do not add it again
            if (await client.GetDocumentAsync($"movie:{movie.Id}") != null) continue;
            
            var doc = new Document($"movie:{movie.Id}");
            doc.Set("title", movie.Title.ToLower());
            await client.AddDocumentAsync(doc);
        }
    }
    
    public async Task GenerateCsvFromRedisHash(string fileName)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var hashKey = "movies_hash";
        
        // Retrieve all hash entries
        var hashEntries = await db.HashGetAllAsync(hashKey);
        var movies = hashEntries
            .Select(entry => JsonConvert.DeserializeObject<MovieSimpleDto>(entry.Value!))
            .ToList();
        var genres = await GetGenresFromRedis();
        
        // Convert to MovieFullDto
        var moviesFull = movies
            .Select(movie => new MovieFullDto(movie!, genres))
            .ToList();
        
        //write only id, title, description, genres
        
        var moviesSimpleCsv = moviesFull
            .Select(movie => new MovieSimpleCsvDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Genres = string.Join(", ", movie.Genres?.Select(genre => genre.Name)!)
            })
            .ToList();
        
        var storagePath = _pyhtonConfiguration.CurrentValue.Storage;
        var storageRealPath = Path.GetFullPath(storagePath);
        // Write data to CSV
        await using (var writer = new StreamWriter(Path.Combine(storageRealPath, fileName)))
        {
            await using (var csv = new CsvWriter(writer,
                             new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
            {
                await csv.WriteRecordsAsync(moviesSimpleCsv);
            }
        }

        _logger.LogInformation("CSV file generated successfully: {FileName}", fileName);
    }
    
    public async Task SaveJsonToRedisHash()
    {
        var storagePat = Path.GetFullPath( _pyhtonConfiguration.CurrentValue.Storage);
        
        var key = "recommendations";
        var db = _connectionMultiplexer.GetDatabase();
        
        var json = await File.ReadAllTextAsync(Path.Combine(storagePat, "movies.json"));
        
        var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double>>>(json);
        
        foreach (var (movieId, recommendations) in data!)
        {
            var movieKey = movieId;
            //get only recommendation keys
            var movieJson = JsonConvert.SerializeObject(recommendations.Keys);
            await db.HashSetAsync(key, movieKey, movieJson);
        }
    }
    
    public async Task<List<MovieStringSimpleDto>> GetRecommendations(int id)
    {
        var key = "recommendations";
        var db = _connectionMultiplexer.GetDatabase();
        
        var recommendationsJson = db.HashGet(key, id);
        
        if (!recommendationsJson.HasValue) return new List<MovieStringSimpleDto>();
        
        var recommendations = JsonConvert.DeserializeObject<List<string>>(recommendationsJson!);
        
        var moviesJson = await db.HashGetAsync("movies_hash", recommendations!.Select(x => (RedisValue)x).ToArray());
        
        var genres = await GetGenresFromRedis();
        
        var movieSimpleDtoList = moviesJson
            .Select(movieJson => JsonConvert.DeserializeObject<MovieSimpleDto>(movieJson!))
            .ToList();
        
        var movieFullDtoList = movieSimpleDtoList
            .Select(movieSimpleDto => new MovieFullDto(movieSimpleDto!, genres))
            .ToList();
        
        var movieStringSimpleDtoList = movieFullDtoList
            .Select(movieFullDto => new MovieStringSimpleDto(movieFullDto, _tmdbConfiguration.CurrentValue.ImageBaseUrl))
            .ToList();
        
        return movieStringSimpleDtoList;

    }
}