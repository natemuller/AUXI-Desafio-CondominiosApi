using Core.Common;
using Core.Models;
using Core.Persistence;
using Core.Repositories.Blocos;
using Microsoft.EntityFrameworkCore;

namespace BlocosApi.Tests.Repositories;

/// <summary>
/// Testa a lógica de paginação e filtros de <see cref="BlocoRepository"/>
/// usando o provider EF Core InMemory (não faz round-trip com o Postgres
/// real). O filtro por "descricao" usa EF.Functions.ILike, que é
/// específico do provider Npgsql e não é exercitado aqui — ver observação
/// no relatório do QA.
/// </summary>
public class BlocoRepositoryTests
{
    private static AuxiDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<AuxiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AuxiDbContext(options);
    }

    private static async Task<AuxiDbContext> CriarContextoComBlocosAsync(
        int codCondom,
        int quantidade)
    {
        var dbContext = CriarContexto();

        for (var i = 1; i <= quantidade; i++)
        {
            dbContext.Blocos.Add(new Bloco
            {
                CodCondom = codCondom,
                CodBloco = $"B{i:D3}",
                Ativo = i % 2 == 0 ? "N" : "S",
            });
        }

        await dbContext.SaveChangesAsync();

        return dbContext;
    }

    [Fact]
    public async Task ListarAsync_PaginaMenorQueUm_EhTratadaComoPaginaUm()
    {
        await using var dbContext = await CriarContextoComBlocosAsync(1, 5);
        var repositorio = new BlocoRepository(dbContext);

        var resultado = await repositorio.ListarAsync(0, null, null, null, null);

        Assert.Equal(1, resultado.PaginaAtual);
    }

    [Fact]
    public async Task ListarAsync_ItensPorPagina_UsaPadraoCentralDeDez()
    {
        await using var dbContext = await CriarContextoComBlocosAsync(1, 25);
        var repositorio = new BlocoRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, null, null, null, null);

        Assert.Equal(PaginationDefaults.ItensPorPagina, resultado.ItensPorPagina);
        Assert.Equal(10, resultado.Items.Count);
    }

    [Theory]
    [InlineData(25, 3)]
    [InlineData(20, 2)]
    [InlineData(21, 3)]
    [InlineData(0, 0)]
    public async Task ListarAsync_CalculaTotalDePaginasCorretamente(
        int totalBlocos,
        int totalPaginasEsperado)
    {
        await using var dbContext = await CriarContextoComBlocosAsync(1, totalBlocos);
        var repositorio = new BlocoRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, null, null, null, null);

        Assert.Equal(totalBlocos, resultado.TotalItens);
        Assert.Equal(totalPaginasEsperado, resultado.TotalPaginas);
    }

    [Fact]
    public async Task ListarAsync_FiltraPorCodCondom()
    {
        await using var dbContext = await CriarContextoComBlocosAsync(1, 3);

        dbContext.Blocos.Add(new Bloco { CodCondom = 2, CodBloco = "X", Ativo = "S" });
        await dbContext.SaveChangesAsync();

        var repositorio = new BlocoRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, codCondom: 2, null, null, null);

        var item = Assert.Single(resultado.Items);
        Assert.Equal(2, item.CodCondom);
        Assert.Equal("X", item.CodBloco);
    }

    [Fact]
    public async Task ListarAsync_FiltraPorCodBlocoExato()
    {
        await using var dbContext = await CriarContextoComBlocosAsync(1, 3);
        var repositorio = new BlocoRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, null, codBloco: "B002", null, null);

        var item = Assert.Single(resultado.Items);
        Assert.Equal("B002", item.CodBloco);
    }

    [Fact]
    public async Task ListarAsync_FiltraPorAtivo()
    {
        await using var dbContext = await CriarContextoComBlocosAsync(1, 4);
        var repositorio = new BlocoRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, null, null, null, ativo: "N");

        Assert.Equal(2, resultado.Items.Count);
        Assert.All(resultado.Items, item => Assert.Equal("N", item.Ativo));
    }

    [Fact]
    public async Task ListarAsync_CodCondomInexistente_RetornaListaVazia()
    {
        await using var dbContext = await CriarContextoComBlocosAsync(1, 3);
        var repositorio = new BlocoRepository(dbContext);

        var resultado = await repositorio.ListarAsync(1, codCondom: 999, null, null, null);

        Assert.Empty(resultado.Items);
        Assert.Equal(0, resultado.TotalItens);
        Assert.Equal(0, resultado.TotalPaginas);
    }

    [Fact]
    public async Task ObterPorChaveAsync_BlocoExistente_RetornaEntidade()
    {
        await using var dbContext = await CriarContextoComBlocosAsync(1, 3);
        var repositorio = new BlocoRepository(dbContext);

        var bloco = await repositorio.ObterPorChaveAsync(1, "B002");

        Assert.NotNull(bloco);
        Assert.Equal("B002", bloco!.CodBloco);
    }

    [Fact]
    public async Task ObterPorChaveAsync_ChaveInexistente_RetornaNull()
    {
        await using var dbContext = await CriarContextoComBlocosAsync(1, 3);
        var repositorio = new BlocoRepository(dbContext);

        var bloco = await repositorio.ObterPorChaveAsync(1, "INEXISTENTE");

        Assert.Null(bloco);
    }

    [Fact]
    public async Task ObterPorChaveAsync_CodCondomCorretoMasCodBlocoDeOutroCondominio_RetornaNull()
    {
        await using var dbContext = await CriarContextoComBlocosAsync(1, 3);

        dbContext.Blocos.Add(new Bloco { CodCondom = 2, CodBloco = "B001", Ativo = "S" });
        await dbContext.SaveChangesAsync();

        var repositorio = new BlocoRepository(dbContext);

        // CodBloco "B001" existe no condomínio 1 e no 2; buscar pela chave
        // errada (condomínio 3) não deve encontrar nada.
        var bloco = await repositorio.ObterPorChaveAsync(3, "B001");

        Assert.Null(bloco);
    }
}
