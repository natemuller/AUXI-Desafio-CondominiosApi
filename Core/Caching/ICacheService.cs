namespace Core.Caching;

public interface ICacheService
{
    Task<(int StatusCode, string Resposta)?> TryGetAsync(
        string chaveCache,
        CancellationToken cancellationToken = default);

    Task SetAsync(
        string chaveCache,
        string urlDaConsulta,
        string metodoHttp,
        string tipoConsulta,
        string entidade,
        int? entidadeId,
        int statusCode,
        string respostaJson,
        CancellationToken cancellationToken = default);
}
