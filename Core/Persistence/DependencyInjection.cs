using Core.Persistence;
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
            configuration.GetConnectionString(ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"A connection string '{ConnectionStringName}' não foi configurada.");
        }

        services.AddDbContext<AuxiDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}