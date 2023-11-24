using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TestcontainersATW.Entities;
using TestcontainersATW.Settings;

namespace TestcontainersATW.Persistence;

public sealed class AzurTechWinterContext : DbContext
{
    private readonly ConnectionStrings _connectionStrings;

    public AzurTechWinterContext(
        DbContextOptions<AzurTechWinterContext> options,
        IOptions<ConnectionStrings> connectionStringsOptions) : base(options)
    {
        _connectionStrings = connectionStringsOptions.Value;
    }

    public DbSet<Player> Players { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionStrings.AzurTechWinter);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Player>(player => { player.HasIndex(x => x.Name).IsUnique(); });
    }
}