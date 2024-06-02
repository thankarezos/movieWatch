using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MovieWatch.Data.Common;
using MovieWatch.Data.Models;

namespace MovieWatch.Data.Database;

public class MovieWatchDbContext : DbContext
{
    private readonly string? _schema;

    public MovieWatchDbContext(DbContextOptions<MovieWatchDbContext> options, IConfiguration configuration) : base(options)
    {
        _schema = configuration.GetConnectionString(name: "Schema");
    }
    
    public DbSet<User> Users { get; set; } 
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        if (!string.IsNullOrWhiteSpace(_schema))
            modelBuilder.HasDefaultSchema(_schema);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var changedEntries = ChangeTracker.Entries()
            .Where(e => e is { Entity: IRecordable, State: EntityState.Added or EntityState.Modified }).ToList();

        var utcNow = DateTime.UtcNow;

        foreach (var entityEntry in changedEntries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                ((IRecordable)entityEntry.Entity).Created = utcNow;
                ((IRecordable)entityEntry.Entity).Updated = utcNow;

                continue;
            }

            ((IRecordable)entityEntry.Entity).Updated = utcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}