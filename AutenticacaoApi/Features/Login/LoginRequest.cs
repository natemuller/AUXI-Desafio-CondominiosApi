namespace AutenticacaoApi.Features.Login;

public sealed record LoginRequest(
    string CpfOuEmail,
    string Senha);
