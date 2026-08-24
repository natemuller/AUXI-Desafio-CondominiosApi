using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Persistence;

public sealed class AuxiDbContext(
    DbContextOptions<AuxiDbContext> options)
    : DbContext(options)
{
    public DbSet<Condominio> Condominios => Set<Condominio>();

    public DbSet<Bloco> Blocos => Set<Bloco>();

    public DbSet<Unidade> Unidades => Set<Unidade>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<UsuarioCredencial> UsuarioCredenciais => Set<UsuarioCredencial>();

    public DbSet<CacheEntrada> CacheEntradas => Set<CacheEntrada>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AuxiDbContext).Assembly);
    }
}