using Core.Caching;

namespace BlocosApi.Features.GetBlocoByChave;

public static class GetBlocoByChaveEndpoint
{
    public static IEndpointRouteBuilder MapGetBlocoByChaveEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/blocos/{codCondom:int}/{codBloco}", async (
                int codCondom,
                string codBloco,
                GetBlocoByChaveHandler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.HandleAsync(
                    codCondom,
                    codBloco,
                    cancellationToken);

                return response is null
                    ? Results.NotFound()
                    : Results.Ok(response);
            })
            .RequireAuthorization()
            .AddEndpointFilter(new ResponseCacheEndpointFilter("bloco", "detalhe"))
            .WithTags("Torres");

        return app;
    }
}
