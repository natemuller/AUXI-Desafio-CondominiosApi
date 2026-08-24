using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Core.Auth;

public sealed class JwtTokenService(IOptions<JwtOptions> jwtOptions)
    : IJwtTokenService
{
    public (string AccessToken, int ExpiresInSeconds) GerarToken(Usuario usuario)
    {
        var options = jwtOptions.Value;

        var signingKey = new SymmetricSecurityKey(
            Convert.FromBase64String(options.SigningKey));

        var credenciais = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new("cpf", usuario.Cpf),
            new(ClaimTypes.Name, usuario.NomeCompleto),
            new("email", usuario.Email),
        };

        var expiraEm = TimeSpan.FromMinutes(options.ExpirationMinutes);
        var expiraEmUtc = DateTime.UtcNow.Add(expiraEm);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: expiraEmUtc,
            signingCredentials: credenciais);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return (accessToken, (int)expiraEm.TotalSeconds);
    }
}
