using Core.Caching;

namespace CondominiosApi.Features.GetByCodCondom;

public static class GetByCodCondomEndpoint
{
    public static IEndpointRouteBuilder MapGetByCodCondomEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/condominios/{codCondom:int}", async (
                int codCondom,
                GetByCodCondomHandler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.HandleAsync(codCondom, cancellationToken);

                return response is null
                    ? Results.NotFound()
                    : Results.Ok(response);
            })
            .RequireAuthorization()
            .AddEndpointFilter(new ResponseCacheEndpointFilter("condominio", "detalhe"))
            .WithTags("Condomínios");

        return app;
    }
}
