using Core.Caching;
using Core.Persistence;
using Core.Repositories.Blocos;
using Core.Repositories.Condominios;
using Core.Repositories.Unidades;
using Core.Repositories.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class DependencyInjection
{
    private const string ConnectionStringName = "SupabaseConnection";

    public static IServiceCollection AddCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' não configurada.");

        services.AddDbContext<AuxiDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<
            ICondominioRepository,
            CondominioRepository>();

        services.AddScoped<
            IBlocoRepository,
            BlocoRepository>();

        services.AddScoped<
            IUnidadeRepository,
            UnidadeRepository>();

        services.AddScoped<
            IUsuarioRepository,
            UsuarioRepository>();

        services.Configure<CacheOptions>(configuration.GetSection("Cache"));

        services.AddScoped<ICacheService, CacheService>();

        return services;
    }
}