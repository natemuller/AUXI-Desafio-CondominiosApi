using Core.Auth;
using Core.Models;

namespace AutenticacaoApi.Tests.TestDoubles;

internal sealed class FakeJwtTokenService : IJwtTokenService
{
    public Usuario? UsuarioRecebido { get; private set; }

    public (string AccessToken, int ExpiresInSeconds) GerarToken(Usuario usuario)
    {
        UsuarioRecebido = usuario;

        return ("token-fake", 3600);
    }
}
