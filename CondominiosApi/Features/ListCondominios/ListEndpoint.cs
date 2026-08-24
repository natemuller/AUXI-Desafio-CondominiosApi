using Core.Caching;

namespace CondominiosApi.Features.ListCondominios;

public static class ListCondominiosEndpoint
{
    public static IEndpointRouteBuilder MapListCondominiosEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/condominios", async (
                [AsParameters] ListRequest request,
                ListHandler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.HandleAsync(request, cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .AddEndpointFilter(new ResponseCacheEndpointFilter("condominio", "lista"))
            .WithTags("Condomínios");

        return app;
    }
}
