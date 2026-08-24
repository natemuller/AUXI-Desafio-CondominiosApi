using Core.Common;
using Core.Models;
using Core.Persistence;
using Core.Repositories.Unidades;
using Microsoft.EntityFrameworkCore;

namespace UnidadesApi.Tests.Repositories;

/// <summary>
/// Testa a lógica de paginação e filtros de <see cref="UnidadeRepository"/>
/// usando o provider EF Core InMemory (não faz round-trip com o Postgres
/// real). O filtro por "nomeCondomino" usa EF.Functions.ILike, que é
/// específico do provider Npgsql e não é exercitado aqui — ver observação
/// no relatório do QA.
/// </summary>
public class UnidadeRepositoryTests
{
    private static AuxiDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<AuxiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AuxiDbContext(options);
    }

    private static async Task<AuxiDbContext> CriarContextoComUnidadesAsync(
        int codCondom,
        string codBloco,
        int quantidade)
    {
        var dbContext = CriarContexto();

        for (var i = 1; i <= quantidade; i++)
        {
            dbContext.Unidades.Add(new Unidade
            {
                Ideconomia = (codCondom * 1000) + i,
                CodCondom = codCondom,
                CodBloco = codBloco,
                CodEconom = $"E{i:D3}",
                TipoUnidade = i % 2 == 0 ? "GARAGEM" : "APARTAMENTO",
                Ativa = i % 3 == 0 ? "N" : "S",
            });
        }

        await dbContext.SaveChangesAsync();

        return dbContext;
    }

    [Fact]
    public async Task ListarAsync_PaginaMenorQueUm_EhTratadaComoPaginaUm()
    {
        await using var dbContext = await CriarContextoComUnidadesAsync(1, "A", 5);
        var repositorio = new UnidadeRepository(dbContext);

        var resultado = await repositorio.ListarAsync(0, null, null, null, null, null, null);

        Assert.Equal(1, resultado.PaginaAtual);
    }

    [Fact]
    public async Task ListarAsync_ItensPorPagina_UsaPadraoCentralDeDez()
    {
        await using var dbContext = await CriarContextoComUnidadesAsync(1, "A", 25);
        var repositorio = new UnidadeRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, null, null, null, null, null, null);

        Assert.Equal(PaginationDefaults.ItensPorPagina, resultado.ItensPorPagina);
        Assert.Equal(10, resultado.Items.Count);
    }

    [Theory]
    [InlineData(25, 3)]
    [InlineData(20, 2)]
    [InlineData(21, 3)]
    [InlineData(0, 0)]
    public async Task ListarAsync_CalculaTotalDePaginasCorretamente(
        int totalUnidades,
        int totalPaginasEsperado)
    {
        await using var dbContext = await CriarContextoComUnidadesAsync(1, "A", totalUnidades);
        var repositorio = new UnidadeRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, null, null, null, null, null, null);

        Assert.Equal(totalUnidades, resultado.TotalItens);
        Assert.Equal(totalPaginasEsperado, resultado.TotalPaginas);
    }

    [Fact]
    public async Task ListarAsync_FiltraPorCodCondomECodBloco()
    {
        await using var dbContext = await CriarContextoComUnidadesAsync(1, "A", 3);

        dbContext.Unidades.Add(new Unidade
        {
            Ideconomia = 9999,
            CodCondom = 1,
            CodBloco = "B",
            CodEconom = "OUTRO",
            Ativa = "S",
        });
        await dbContext.SaveChangesAsync();

        var repositorio = new UnidadeRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, codCondom: 1, codBloco: "B", null, null, null, null);

        var item = Assert.Single(resultado.Items);
        Assert.Equal("OUTRO", item.CodEconom);
    }

    [Fact]
    public async Task ListarAsync_FiltraPorTipoUnidade()
    {
        await using var dbContext = await CriarContextoComUnidadesAsync(1, "A", 4);
        var repositorio = new UnidadeRepository(dbContext);

        var resultado = await repositorio.ListarAsync(
            1, null, null, null, tipoUnidade: "GARAGEM", null, null);

        Assert.Equal(2, resultado.Items.Count);
        Assert.All(resultado.Items, item => Assert.Equal("GARAGEM", item.TipoUnidade));
    }

    [Fact]
    public async Task ListarAsync_FiltraPorAtiva()
    {
        await using var dbContext = await CriarContextoComUnidadesAsync(1, "A", 6);
        var repositorio = new UnidadeRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, null, null, null, null, ativa: "N", null);

        Assert.Equal(2, resultado.Items.Count);
        Assert.All(resultado.Items, item => Assert.Equal("N", item.Ativa));
    }

    [Fact]
    public async Task ListarAsync_FiltraPorCodEconomExato()
    {
        await using var dbContext = await CriarContextoComUnidadesAsync(1, "A", 3);
        var repositorio = new UnidadeRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, null, null, codEconom: "E002", null, null, null);

        var item = Assert.Single(resultado.Items);
        Assert.Equal("E002", item.CodEconom);
    }

    [Fact]
    public async Task ListarAsync_CodCondomInexistente_RetornaListaVazia()
    {
        await using var dbContext = await CriarContextoComUnidadesAsync(1, "A", 3);
        var repositorio = new UnidadeRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, codCondom: 999, null, null, null, null, null);

        Assert.Empty(resultado.Items);
        Assert.Equal(0, resultado.TotalItens);
        Assert.Equal(0, resultado.TotalPaginas);
    }

    [Fact]
    public async Task ObterPorIdAsync_UnidadeExistente_RetornaEntidade()
    {
        await using var dbContext = await CriarContextoComUnidadesAsync(1, "A", 3);
        var repositorio = new UnidadeRepository(dbContext);

        var unidade = await repositorio.ObterPorIdAsync(1002);

        Assert.NotNull(unidade);
        Assert.Equal("E002", unidade!.CodEconom);
    }

    [Fact]
    public async Task ObterPorIdAsync_IdInexistente_RetornaNull()
    {
        await using var dbContext = await CriarContextoComUnidadesAsync(1, "A", 3);
        var repositorio = new UnidadeRepository(dbContext);

        var unidade = await repositorio.ObterPorIdAsync(999999);

        Assert.Null(unidade);
    }
}
