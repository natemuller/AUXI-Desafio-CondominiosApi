using Core.Caching;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CondominiosApi.Tests.Caching;

/// <summary>
/// Testa <see cref="CacheService"/> contra um <see cref="AuxiDbContext"/> em
/// memória (EF Core InMemory), já que a dependência é a classe concreta do
/// DbContext, e não uma interface.
/// </summary>
public class CacheServiceTests
{
    private static AuxiDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<AuxiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AuxiDbContext(options);
    }

    [Fact]
    public async Task TryGetAsync_QuandoNaoHaEntradaParaAChave_RetornaNull()
    {
        await using var dbContext = CriarContexto();
        var cacheService = new CacheService(dbContext, Options.Create(new CacheOptions()));

        var resultado = await cacheService.TryGetAsync("chave-inexistente");

        Assert.Null(resultado);
    }

    [Fact]
    public async Task SetAsync_GravaEntrada_QueEhLidaDepoisPorTryGetAsync()
    {
        await using var dbContext = CriarContexto();
        var cacheService = new CacheService(
            dbContext,
            Options.Create(new CacheOptions { TtlSeconds = 300 }));

        await cacheService.SetAsync(
            "chave-1",
            "/api/condominios",
            "GET",
            "lista",
            "condominio",
            null,
            200,
            "{\"foo\":\"bar\"}");

        var resultado = await cacheService.TryGetAsync("chave-1");

        Assert.NotNull(resultado);
        Assert.Equal(200, resultado!.Value.StatusCode);
        Assert.Equal("{\"foo\":\"bar\"}", resultado.Value.Resposta);
    }

    [Fact]
    public async Task TryGetAsync_EntradaExpiradaPeloTtl_NaoEhRetornada()
    {
        await using var dbContext = CriarContexto();
        var cacheService = new CacheService(
            dbContext,
            Options.Create(new CacheOptions { TtlSeconds = -1 }));

        await cacheService.SetAsync(
            "chave-expirada",
            "/api/condominios",
            "GET",
            "lista",
            "condominio",
            null,
            200,
            "{}");

        var resultado = await cacheService.TryGetAsync("chave-expirada");

        Assert.Null(resultado);
    }

    [Fact]
    public async Task TryGetAsync_EntradaInvalidada_NaoEhRetornada()
    {
        await using var dbContext = CriarContexto();
        var cacheService = new CacheService(
            dbContext,
            Options.Create(new CacheOptions { TtlSeconds = 300 }));

        await cacheService.SetAsync(
            "chave-invalidada",
            "/api/condominios",
            "GET",
            "lista",
            "condominio",
            null,
            200,
            "{}");

        var entrada = await dbContext.CacheEntradas.SingleAsync();
        entrada.InvalidadoEm = DateTimeOffset.UtcNow;
        entrada.MotivoInvalidacao = "teste";
        await dbContext.SaveChangesAsync();

        var resultado = await cacheService.TryGetAsync("chave-invalidada");

        Assert.Null(resultado);
    }

    [Fact]
    public async Task TryGetAsync_ChaveDiferente_NaoRetornaEntradaDeOutraChave()
    {
        await using var dbContext = CriarContexto();
        var cacheService = new CacheService(
            dbContext,
            Options.Create(new CacheOptions { TtlSeconds = 300 }));

        await cacheService.SetAsync(
            "chave-a",
            "/api/condominios",
            "GET",
            "lista",
            "condominio",
            null,
            200,
            "{}");

        var resultado = await cacheService.TryGetAsync("chave-b");

        Assert.Null(resultado);
    }
}
