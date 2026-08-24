using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Core.Caching;

/// <summary>
/// Filtro de endpoint que consulta/grava respostas em cache (tabela
/// "cache") usando a URL + querystring da requisição como chave.
/// </summary>
public sealed class ResponseCacheEndpointFilter(
    string entidade,
    string tipoConsulta)
    : IEndpointFilter
{
    private const int TamanhoMaximoChave = 500;
    private const int StatusCodeSucessoPadrao = 200;

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var chaveCache = MontarChaveCache(httpContext.Request);

        var cacheService = httpContext.RequestServices
            .GetRequiredService<ICacheService>();

        var cacheado = await cacheService.TryGetAsync(
            chaveCache,
            httpContext.RequestAborted);

        if (cacheado is not null)
        {
            var (statusCode, resposta) = cacheado.Value;

            using var jsonDocument = JsonDocument.Parse(resposta);

            return Results.Json(
                jsonDocument.RootElement.Clone(),
                statusCode: statusCode);
        }

        var result = await next(context);

        var statusCodeResultado = result is IStatusCodeHttpResult statusCodeHttpResult
            ? statusCodeHttpResult.StatusCode ?? StatusCodeSucessoPadrao
            : StatusCodeSucessoPadrao;

        if (statusCodeResultado is >= 200 and < 300 &&
            result is IValueHttpResult valueHttpResult &&
            valueHttpResult.Value is not null)
        {
            // Usa as mesmas JsonSerializerOptions do pipeline de minimal APIs
            // (camelCase por padrão) para que a resposta cacheada fique
            // idêntica à resposta não cacheada.
            var jsonOptions = httpContext.RequestServices
                .GetRequiredService<IOptions<JsonOptions>>()
                .Value
                .SerializerOptions;

            var respostaJson = JsonSerializer.Serialize(
                valueHttpResult.Value,
                jsonOptions);

            await cacheService.SetAsync(
                chaveCache,
                httpContext.Request.Path + httpContext.Request.QueryString,
                httpContext.Request.Method,
                tipoConsulta,
                entidade,
                entidadeId: null,
                statusCodeResultado,
                respostaJson,
                httpContext.RequestAborted);
        }

        return result;
    }

    private static string MontarChaveCache(HttpRequest request)
    {
        var chave = $"{request.Path}{request.QueryString}";

        return chave.Length > TamanhoMaximoChave
            ? chave[..TamanhoMaximoChave]
            : chave;
    }
}
