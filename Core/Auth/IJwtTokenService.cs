using Core.Models;

namespace Core.Auth;

public interface IJwtTokenService
{
    (string AccessToken, int ExpiresInSeconds) GerarToken(Usuario usuario);
}
