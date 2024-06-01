using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieWatch.Services.Extensions;
using MovieWatch.Data.Database;
using MovieWatch.Services.Services;
using Newtonsoft.Json;
using StackExchange.Redis;
using MovieWatch.Data.Models;
using FluentValidation;
using MovieWatch.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MovieWatchDbContext>(options =>
    options.UseNpgsql(connectionString: builder.Configuration.GetConnectionString(name: "DefaultConnection"),
            x => x.MigrationsHistoryTable(tableName: "migrations_history",
                schema: builder.Configuration.GetConnectionString(name: "Schema")))
        .UseSnakeCaseNamingConvention());
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

//Controller & responses behaviour
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = BaseFirstContractResolver.Instance;
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });

builder.Services.AddTransient<IWeatherService, WeatherService>();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
ValidatorOptions.Global.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Add row to the database
    // using var scope = app.Services.CreateScope();
    // var dbContext = scope.ServiceProvider.GetRequiredService<MovieWatchDbContext>();
    // dbContext.WeatherForecasts.Add(new WeatherForecast
    // {
    //     TemperatureC = 20,
    //     Summary = "Sunny"
    // });
    //
    // await dbContext.SaveChangesAsync();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
