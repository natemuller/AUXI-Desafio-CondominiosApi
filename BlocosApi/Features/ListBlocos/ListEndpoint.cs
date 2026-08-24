using Core.Caching;

namespace BlocosApi.Features.ListBlocos;

public static class ListBlocosEndpoint
{
    public static IEndpointRouteBuilder MapListBlocosEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/blocos", async (
                [AsParameters] ListRequest request,
                ListHandler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.HandleAsync(request, cancellationToken);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .AddEndpointFilter(new ResponseCacheEndpointFilter("bloco", "lista"))
            .WithTags("Torres");

        return app;
    }
}
