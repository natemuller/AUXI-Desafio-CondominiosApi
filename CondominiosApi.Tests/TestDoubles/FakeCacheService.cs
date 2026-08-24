using Core.Caching;

namespace CondominiosApi.Tests.TestDoubles;

/// <summary>
/// Dublê manual de <see cref="ICacheService"/> usado para testar o
/// <see cref="ResponseCacheEndpointFilter"/> sem depender de EF Core / Postgres.
/// </summary>
internal sealed class FakeCacheService : ICacheService
{
    public (int StatusCode, string Resposta)? ValorParaRetornar { get; set; }

    public bool SetAsyncFoiChamado { get; private set; }

    public string? UltimaChaveCache { get; private set; }

    public string? UltimaEntidade { get; private set; }

    public string? UltimoTipoConsulta { get; private set; }

    public int? UltimoStatusCode { get; private set; }

    public string? UltimaRespostaJson { get; private set; }

    public Task<(int StatusCode, string Resposta)?> TryGetAsync(
        string chaveCache,
        CancellationToken cancellationToken = default)
    {
        UltimaChaveCache = chaveCache;

        return Task.FromResult(ValorParaRetornar);
    }

    public Task SetAsync(
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
        SetAsyncFoiChamado = true;
        UltimaChaveCache = chaveCache;
        UltimaEntidade = entidade;
        UltimoTipoConsulta = tipoConsulta;
        UltimoStatusCode = statusCode;
        UltimaRespostaJson = respostaJson;

        return Task.CompletedTask;
    }
}
