using Core.Auth;
using Core.Repositories.Usuarios;
using Microsoft.Extensions.Options;

namespace AutenticacaoApi.Features.Login;

public sealed class LoginHandler(
    IUsuarioRepository usuarioRepository,
    IJwtTokenService jwtTokenService,
    IOptions<DevCredentialOptions> devCredentialOptions)
{
    public async Task<LoginResponse?> HandleAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var encontrado = await usuarioRepository.ObterPorCpfOuEmailAsync(
            request.CpfOuEmail,
            cancellationToken);

        if (encontrado is null)
        {
            return null;
        }

        var (usuario, credencial) = encontrado.Value;

        var autenticado = EhCredencialDeDesenvolvimentoValida(request)
            || VerificarSenha(request.Senha, credencial.SenhaHash);

        if (!autenticado)
        {
            return null;
        }

        var (accessToken, expiresInSeconds) = jwtTokenService.GerarToken(usuario);

        return new LoginResponse(
            accessToken,
            expiresInSeconds,
            new UsuarioResponse(
                usuario.Id,
                usuario.Cpf,
                usuario.NomeCompleto,
                usuario.Email));
    }

    private bool EhCredencialDeDesenvolvimentoValida(LoginRequest request)
    {
        var opcoes = devCredentialOptions.Value;

        if (!opcoes.Enabled)
        {
            return false;
        }

        var cpfNormalizado = new string(
            request.CpfOuEmail.Where(char.IsDigit).ToArray());

        return cpfNormalizado == opcoes.Cpf
            && request.Senha == opcoes.Password;
    }

    private static bool VerificarSenha(string senha, string senhaHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, senhaHash);
        }
        catch
        {
            return false;
        }
    }
}
