using Core.Caching;

namespace UnidadesApi.Features.GetUnidadeById;

public static class GetUnidadeByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetUnidadeByIdEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/unidades/{ideconomia:int}", async (
                int ideconomia,
                GetUnidadeByIdHandler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.HandleAsync(ideconomia, cancellationToken);

                return response is null
                    ? Results.NotFound()
                    : Results.Ok(response);
            })
            .RequireAuthorization()
            .AddEndpointFilter(new ResponseCacheEndpointFilter("unidade", "detalhe"))
            .WithTags("Unidades");

        return app;
    }
}
