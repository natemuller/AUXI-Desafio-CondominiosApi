namespace AutenticacaoApi.Features.Login;

public sealed record LoginResponse(
    string AccessToken,
    int ExpiresInSeconds,
    UsuarioResponse Usuario)
{
    public string TokenType { get; init; } = "Bearer";
}

public sealed record UsuarioResponse(
    Guid Id,
    string Cpf,
    string NomeCompleto,
    string Email);
