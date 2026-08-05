using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Persistence;

public sealed class AuxiDbContext(
    DbContextOptions<AuxiDbContext> options)
    : DbContext(options)
{
    public DbSet<Condominio> Condominios => Set<Condominio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AuxiDbContext).Assembly);
    }
}