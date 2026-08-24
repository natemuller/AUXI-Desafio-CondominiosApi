using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Core.Auth;
using Core.Models;
using Microsoft.Extensions.Options;

namespace AutenticacaoApi.Tests.Auth;

public class JwtTokenServiceTests
{
    private static JwtOptions CriarOpcoes(int expirationMinutes = 60) => new()
    {
        SigningKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
        Issuer = "AuxiDesafio.Testes",
        Audience = "AuxiDesafio.Api",
        ExpirationMinutes = expirationMinutes,
    };

    private static Usuario CriarUsuario() => new()
    {
        Id = Guid.NewGuid(),
        Cpf = "12345678900",
        NomeCompleto = "Usuário de Teste",
        Email = "usuario@teste.com",
        Status = "ativo",
        CriadoEm = DateTimeOffset.UtcNow,
        AtualizadoEm = DateTimeOffset.UtcNow,
    };

    [Fact]
    public void GerarToken_GeraTokenValidoComClaimsEsperadas()
    {
        var opcoes = CriarOpcoes();
        var usuario = CriarUsuario();
        var service = new JwtTokenService(Options.Create(opcoes));

        var (accessToken, expiresInSeconds) = service.GerarToken(usuario);

        Assert.False(string.IsNullOrWhiteSpace(accessToken));

        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(accessToken));

        var token = handler.ReadJwtToken(accessToken);

        Assert.Equal(opcoes.Issuer, token.Issuer);
        Assert.Contains(opcoes.Audience, token.Audiences);
        Assert.Equal(
            usuario.Id.ToString(),
            token.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(usuario.Cpf, token.Claims.Single(c => c.Type == "cpf").Value);
        Assert.Equal(usuario.Email, token.Claims.Single(c => c.Type == "email").Value);
        Assert.Equal(
            usuario.NomeCompleto,
            token.Claims.Single(c => c.Type == ClaimTypes.Name).Value);
        Assert.Equal(3600, expiresInSeconds);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(120)]
    public void GerarToken_RespeitaExpirationMinutesConfigurado(int expirationMinutes)
    {
        var opcoes = CriarOpcoes(expirationMinutes);
        var usuario = CriarUsuario();
        var service = new JwtTokenService(Options.Create(opcoes));

        var antes = DateTime.UtcNow;
        var (accessToken, expiresInSeconds) = service.GerarToken(usuario);

        Assert.Equal(expirationMinutes * 60, expiresInSeconds);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        var expiraEmEsperado = antes.AddMinutes(expirationMinutes);

        Assert.True(
            Math.Abs((token.ValidTo - expiraEmEsperado).TotalSeconds) < 5,
            $"Esperado expirar próximo de {expiraEmEsperado:o}, mas expira em {token.ValidTo:o}");
    }

    [Fact]
    public void GerarToken_UsuariosDiferentes_GeramTokensComSubClaimDiferente()
    {
        var opcoes = CriarOpcoes();
        var service = new JwtTokenService(Options.Create(opcoes));

        var usuario1 = CriarUsuario();
        var usuario2 = CriarUsuario();

        var (token1, _) = service.GerarToken(usuario1);
        var (token2, _) = service.GerarToken(usuario2);

        var handler = new JwtSecurityTokenHandler();
        var sub1 = handler.ReadJwtToken(token1)
            .Claims.Single(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
        var sub2 = handler.ReadJwtToken(token2)
            .Claims.Single(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

        Assert.NotEqual(sub1, sub2);
        Assert.Equal(usuario1.Id.ToString(), sub1);
        Assert.Equal(usuario2.Id.ToString(), sub2);
    }
}
