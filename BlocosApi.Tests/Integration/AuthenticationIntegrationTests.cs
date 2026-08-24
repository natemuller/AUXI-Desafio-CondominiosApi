using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BlocosApi.Tests.Integration;

/// <summary>
/// Teste de integração leve usando WebApplicationFactory (sem tocar o
/// Postgres real): configura uma connection string sintaticamente válida
/// (mas nunca contatada, pois a requisição sem token é barrada antes de
/// qualquer acesso a dados) e uma chave de assinatura JWT descartável, só
/// para permitir o start-up do host em ambiente de teste.
/// </summary>
public class AuthenticationIntegrationTests
{
    private const string ConnectionStringEnvVar = "ConnectionStrings__SupabaseConnection";
    private const string SigningKeyEnvVar = "Jwt__SigningKey";

    [Fact]
    public async Task GetBlocos_SemHeaderDeAutorizacao_Retorna401()
    {
        var connectionStringOriginal = Environment.GetEnvironmentVariable(ConnectionStringEnvVar);
        var signingKeyOriginal = Environment.GetEnvironmentVariable(SigningKeyEnvVar);

        Environment.SetEnvironmentVariable(
            ConnectionStringEnvVar,
            "Host=localhost;Port=5432;Database=testes_qa;Username=testes_qa;Password=testes_qa");
        Environment.SetEnvironmentVariable(
            SigningKeyEnvVar,
            Convert.ToBase64String(new byte[32]));

        try
        {
            await using var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var resposta = await client.GetAsync("/api/blocos");

            Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        }
        finally
        {
            Environment.SetEnvironmentVariable(ConnectionStringEnvVar, connectionStringOriginal);
            Environment.SetEnvironmentVariable(SigningKeyEnvVar, signingKeyOriginal);
        }
    }
}
