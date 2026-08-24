using Core.Models;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Caching;

public sealed class CacheService(
    AuxiDbContext dbContext,
    IOptions<CacheOptions> cacheOptions)
    : ICacheService
{
    public async Task<(int StatusCode, string Resposta)?> TryGetAsync(
        string chaveCache,
        CancellationToken cancellationToken = default)
    {
        var agora = DateTimeOffset.UtcNow;

        var entrada = await dbContext.CacheEntradas
            .AsNoTracking()
            .Where(cache =>
                cache.ChaveCache == chaveCache &&
                cache.InvalidadoEm == null &&
                cache.ExpiradoEm > agora)
            .OrderByDescending(cache => cache.CriadoEm)
            .FirstOrDefaultAsync(cancellationToken);

        if (entrada is null)
        {
            return null;
        }

        return (entrada.StatusCode, entrada.Resposta);
    }

    public async Task SetAsync(
        string chaveCache,
        string urlDaConsulta,
        string metodoHttp,
        string tipoConsulta,
        string entidade,
        int? entidadeId,
        int statusCode,
        string respostaJson,
        CancellationToken cancellationToken = default)
    {
        var agora = DateTimeOffset.UtcNow;

        var entrada = new CacheEntrada
        {
            Id = Guid.NewGuid(),
            ChaveCache = chaveCache,
            UrlDaConsulta = urlDaConsulta,
            MetodoHttp = metodoHttp,
            TipoConsulta = tipoConsulta,
            Entidade = entidade,
            EntidadeId = entidadeId,
            Resposta = respostaJson,
            StatusCode = statusCode,
            CriadoEm = agora,
            ExpiradoEm = agora.AddSeconds(cacheOptions.Value.TtlSeconds),
        };

        dbContext.CacheEntradas.Add(entrada);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
