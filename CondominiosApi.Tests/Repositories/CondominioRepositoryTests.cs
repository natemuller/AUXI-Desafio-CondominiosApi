using Core.Common;
using Core.Models;
using Core.Persistence;
using Core.Repositories.Condominios;
using Microsoft.EntityFrameworkCore;

namespace CondominiosApi.Tests.Repositories;

/// <summary>
/// Testa a lógica de paginação e filtros de <see cref="CondominioRepository"/>
/// usando o provider EF Core InMemory (não faz round-trip com o Postgres
/// real). Filtros baseados em EF.Functions.ILike (ex.: "nome") são
/// específicos do provider Npgsql e não são exercitados aqui — ver
/// observação no relatório do QA.
/// </summary>
public class CondominioRepositoryTests
{
    private static AuxiDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<AuxiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AuxiDbContext(options);
    }

    private static async Task<AuxiDbContext> CriarContextoComCondominiosAsync(int quantidade)
    {
        var dbContext = CriarContexto();

        for (var i = 1; i <= quantidade; i++)
        {
            dbContext.Condominios.Add(new Condominio
            {
                CodCondom = i,
                NomeCondom = $"Condominio {i:D3}",
                Cnpj = $"{i:D14}",
            });
        }

        await dbContext.SaveChangesAsync();

        return dbContext;
    }

    [Fact]
    public async Task ListarAsync_PaginaMenorQueUm_EhTratadaComoPaginaUm()
    {
        await using var dbContext = await CriarContextoComCondominiosAsync(5);
        var repositorio = new CondominioRepository(dbContext);

        var resultado = await repositorio.ListarAsync(0, null, null, null);

        Assert.Equal(1, resultado.PaginaAtual);
    }

    [Fact]
    public async Task ListarAsync_PaginaNegativa_EhTratadaComoPaginaUm()
    {
        await using var dbContext = await CriarContextoComCondominiosAsync(5);
        var repositorio = new CondominioRepository(dbContext);

        var resultado = await repositorio.ListarAsync(-10, null, null, null);

        Assert.Equal(1, resultado.PaginaAtual);
    }

    [Fact]
    public async Task ListarAsync_ItensPorPagina_UsaPadraoCentralDeDez()
    {
        await using var dbContext = await CriarContextoComCondominiosAsync(25);
        var repositorio = new CondominioRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, null, null, null);

        Assert.Equal(PaginationDefaults.ItensPorPagina, resultado.ItensPorPagina);
        Assert.Equal(10, resultado.Items.Count);
    }

    [Theory]
    [InlineData(25, 3)]
    [InlineData(20, 2)]
    [InlineData(21, 3)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    public async Task ListarAsync_CalculaTotalDePaginasCorretamente(
        int totalCondominios,
        int totalPaginasEsperado)
    {
        await using var dbContext = await CriarContextoComCondominiosAsync(totalCondominios);
        var repositorio = new CondominioRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, null, null, null);

        Assert.Equal(totalCondominios, resultado.TotalItens);
        Assert.Equal(totalPaginasEsperado, resultado.TotalPaginas);
    }

    [Fact]
    public async Task ListarAsync_UltimaPagina_RetornaResiduoDeItens()
    {
        await using var dbContext = await CriarContextoComCondominiosAsync(25);
        var repositorio = new CondominioRepository(dbContext);

        var resultado = await repositorio.ListarAsync(3, null, null, null);

        Assert.Equal(3, resultado.PaginaAtual);
        Assert.Equal(5, resultado.Items.Count);
    }

    [Fact]
    public async Task ListarAsync_PaginaAlemDoTotal_RetornaListaVazia()
    {
        await using var dbContext = await CriarContextoComCondominiosAsync(5);
        var repositorio = new CondominioRepository(dbContext);

        var resultado = await repositorio.ListarAsync(99, null, null, null);

        Assert.Empty(resultado.Items);
        Assert.Equal(5, resultado.TotalItens);
        Assert.Equal(99, resultado.PaginaAtual);
    }

    [Fact]
    public async Task ListarAsync_FiltraPorCodCondom()
    {
        await using var dbContext = await CriarContextoComCondominiosAsync(10);
        var repositorio = new CondominioRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, null, codCondom: 5, nome: null);

        var item = Assert.Single(resultado.Items);
        Assert.Equal(5, item.CodCondom);
    }

    [Fact]
    public async Task ListarAsync_FiltraPorCnpjExato()
    {
        await using var dbContext = await CriarContextoComCondominiosAsync(3);
        var repositorio = new CondominioRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, cnpj: $"{2:D14}", codCondom: null, nome: null);

        var item = Assert.Single(resultado.Items);
        Assert.Equal(2, item.CodCondom);
    }

    [Fact]
    public async Task ListarAsync_CodigoInexistente_RetornaListaVaziaComTotalZero()
    {
        await using var dbContext = await CriarContextoComCondominiosAsync(3);
        var repositorio = new CondominioRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, null, codCondom: 999, nome: null);

        Assert.Empty(resultado.Items);
        Assert.Equal(0, resultado.TotalItens);
        Assert.Equal(0, resultado.TotalPaginas);
    }

    [Fact]
    public async Task ObterPorCodigoAsync_CondominioExistente_RetornaEntidade()
    {
        await using var dbContext = await CriarContextoComCondominiosAsync(3);
        var repositorio = new CondominioRepository(dbContext);

        var condominio = await repositorio.ObterPorCodigoAsync(2);

        Assert.NotNull(condominio);
        Assert.Equal(2, condominio!.CodCondom);
    }

    [Fact]
    public async Task ObterPorCodigoAsync_CodigoInexistente_RetornaNull()
    {
        await using var dbContext = await CriarContextoComCondominiosAsync(3);
        var repositorio = new CondominioRepository(dbContext);

        var condominio = await repositorio.ObterPorCodigoAsync(999);

        Assert.Null(condominio);
    }
}
