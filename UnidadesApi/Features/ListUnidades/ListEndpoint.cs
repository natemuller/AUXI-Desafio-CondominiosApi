using Core.Caching;

namespace UnidadesApi.Features.ListUnidades;

public static class ListUnidadesEndpoint
{
    public static IEndpointRouteBuilder MapListUnidadesEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/unidades", async (
                [AsParameters] ListRequest request,
                ListHandler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.HandleAsync(request, cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .AddEndpointFilter(new ResponseCacheEndpointFilter("unidade", "lista"))
            .WithTags("Unidades");

        return app;
    }
}
