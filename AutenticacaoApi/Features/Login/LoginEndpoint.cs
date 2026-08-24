namespace AutenticacaoApi.Features.Login;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
                LoginRequest request,
                LoginHandler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.HandleAsync(request, cancellationToken);

                return response is null
                    ? Results.Json(
                        new { message = "CPF/e-mail ou senha inválidos." },
                        statusCode: StatusCodes.Status401Unauthorized)
                    : Results.Ok(response);
            })
            .AllowAnonymous()
            .WithTags("Autenticação");

        return app;
    }
}
