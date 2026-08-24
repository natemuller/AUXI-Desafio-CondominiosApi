using AutenticacaoApi.Features.Login;
using AutenticacaoApi.Tests.TestDoubles;
using Core.Auth;
using Core.Models;
using Microsoft.Extensions.Options;

namespace AutenticacaoApi.Tests.Features.Login;

public class LoginHandlerTests
{
    private static Usuario CriarUsuario(
        string cpf = "12345678900",
        string email = "usuario@teste.com") => new()
    {
        Id = Guid.NewGuid(),
        Cpf = cpf,
        NomeCompleto = "Usuário Teste",
        Email = email,
        Status = "ativo",
        CriadoEm = DateTimeOffset.UtcNow,
        AtualizadoEm = DateTimeOffset.UtcNow,
    };

    [Fact]
    public async Task HandleAsync_UsuarioNaoEncontrado_RetornaNull()
    {
        var usuarioRepository = new FakeUsuarioRepository { ResultadoParaRetornar = null };
        var handler = new LoginHandler(
            usuarioRepository,
            new FakeJwtTokenService(),
            Options.Create(new DevCredentialOptions()));

        var resposta = await handler.HandleAsync(
            new LoginRequest("000.000.000-00", "qualquer"),
            CancellationToken.None);

        Assert.Null(resposta);
    }

    [Fact]
    public async Task HandleAsync_SenhaIncorreta_RetornaNull()
    {
        var usuario = CriarUsuario();
        var credencial = new UsuarioCredencial
        {
            UsuarioId = usuario.Id,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("SenhaCorreta123"),
        };

        var usuarioRepository = new FakeUsuarioRepository
        {
            ResultadoParaRetornar = (usuario, credencial),
        };

        var handler = new LoginHandler(
            usuarioRepository,
            new FakeJwtTokenService(),
            Options.Create(new DevCredentialOptions()));

        var resposta = await handler.HandleAsync(
            new LoginRequest(usuario.Cpf, "SenhaErrada"),
            CancellationToken.None);

        Assert.Null(resposta);
    }

    [Fact]
    public async Task HandleAsync_SenhaCorretaViaBCrypt_AutenticaERetornaToken()
    {
        var usuario = CriarUsuario();
        var credencial = new UsuarioCredencial
        {
            UsuarioId = usuario.Id,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("SenhaCorreta123"),
        };

        var usuarioRepository = new FakeUsuarioRepository
        {
            ResultadoParaRetornar = (usuario, credencial),
        };

        var jwtTokenService = new FakeJwtTokenService();
        var handler = new LoginHandler(
            usuarioRepository,
            jwtTokenService,
            Options.Create(new DevCredentialOptions()));

        var resposta = await handler.HandleAsync(
            new LoginRequest(usuario.Cpf, "SenhaCorreta123"),
            CancellationToken.None);

        Assert.NotNull(resposta);
        Assert.Equal("token-fake", resposta!.AccessToken);
        Assert.Equal(3600, resposta.ExpiresInSeconds);
        Assert.Equal("Bearer", resposta.TokenType);
        Assert.Equal(usuario.Id, resposta.Usuario.Id);
        Assert.Equal(usuario.Cpf, resposta.Usuario.Cpf);
        Assert.Equal(usuario.Email, resposta.Usuario.Email);
        Assert.Equal(usuario.Id, jwtTokenService.UsuarioRecebido?.Id);
    }

    [Fact]
    public async Task HandleAsync_AtalhoDeDesenvolvimentoHabilitado_AutenticaMesmoComHashInvalido()
    {
        var usuario = CriarUsuario(cpf: "98765432100");
        var credencial = new UsuarioCredencial
        {
            UsuarioId = usuario.Id,
            SenhaHash = "hash-invalido-que-nunca-bate",
        };

        var usuarioRepository = new FakeUsuarioRepository
        {
            ResultadoParaRetornar = (usuario, credencial),
        };

        var devCredentialOptions = Options.Create(new DevCredentialOptions
        {
            Enabled = true,
            Cpf = "98765432100",
            Password = "dev-password",
        });

        var handler = new LoginHandler(usuarioRepository, new FakeJwtTokenService(), devCredentialOptions);

        var resposta = await handler.HandleAsync(
            new LoginRequest("987.654.321-00", "dev-password"),
            CancellationToken.None);

        Assert.NotNull(resposta);
    }

    [Fact]
    public async Task HandleAsync_AtalhoDeDesenvolvimentoDesabilitado_NaoAutenticaComSenhaDeAtalho()
    {
        var usuario = CriarUsuario(cpf: "98765432100");
        var credencial = new UsuarioCredencial
        {
            UsuarioId = usuario.Id,
            SenhaHash = "hash-invalido-que-nunca-bate",
        };

        var usuarioRepository = new FakeUsuarioRepository
        {
            ResultadoParaRetornar = (usuario, credencial),
        };

        var devCredentialOptions = Options.Create(new DevCredentialOptions
        {
            Enabled = false,
            Cpf = "98765432100",
            Password = "dev-password",
        });

        var handler = new LoginHandler(usuarioRepository, new FakeJwtTokenService(), devCredentialOptions);

        var resposta = await handler.HandleAsync(
            new LoginRequest("987.654.321-00", "dev-password"),
            CancellationToken.None);

        Assert.Null(resposta);
    }

    [Fact]
    public async Task HandleAsync_AtalhoDeDesenvolvimentoComSenhaErrada_RetornaNull()
    {
        var usuario = CriarUsuario(cpf: "98765432100");
        var credencial = new UsuarioCredencial
        {
            UsuarioId = usuario.Id,
            SenhaHash = "hash-invalido-que-nunca-bate",
        };

        var usuarioRepository = new FakeUsuarioRepository
        {
            ResultadoParaRetornar = (usuario, credencial),
        };

        var devCredentialOptions = Options.Create(new DevCredentialOptions
        {
            Enabled = true,
            Cpf = "98765432100",
            Password = "dev-password",
        });

        var handler = new LoginHandler(usuarioRepository, new FakeJwtTokenService(), devCredentialOptions);

        var resposta = await handler.HandleAsync(
            new LoginRequest("987.654.321-00", "senha-errada"),
            CancellationToken.None);

        Assert.Null(resposta);
    }
}
