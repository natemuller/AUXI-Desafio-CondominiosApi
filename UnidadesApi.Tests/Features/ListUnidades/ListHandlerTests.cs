using Core.Common;
using Core.Models;
using UnidadesApi.Features.ListUnidades;
using UnidadesApi.Tests.TestDoubles;

namespace UnidadesApi.Tests.Features.ListUnidades;

public class ListHandlerTests
{
    [Fact]
    public async Task HandleAsync_RepassaFiltrosParaORepositorio()
    {
        var repositorio = new FakeUnidadeRepository();
        var handler = new ListHandler(repositorio);
        var request = new ListRequest(
            CodCondom: 42,
            CodBloco: "A",
            CodEconom: "101",
            TipoUnidade: "AP",
            Ativa: "S",
            NomeCondomino: "Fulano",
            Pagina: 2);

        await handler.HandleAsync(request, CancellationToken.None);

        Assert.Equal(
            (2, (int?)42, "A", "101", "AP", "S", "Fulano"),
            repositorio.UltimaChamadaListar);
    }

    [Fact]
    public async Task HandleAsync_MapeiaItensEPaginacaoCorretamente()
    {
        var unidade = new Unidade
        {
            Ideconomia = 100,
            CodCondom = 10,
            CodBloco = "B1",
            CodEconom = "101",
        };

        var repositorio = new FakeUnidadeRepository
        {
            ResultadoListar = new PagedResult<Unidade>([unidade], 2, 10, 15, 2),
        };

        var handler = new ListHandler(repositorio);

        var response = await handler.HandleAsync(
            new ListRequest(null, null, null, null, null, null),
            CancellationToken.None);

        var item = Assert.Single(response.Items);
        Assert.Equal(100, item.Ideconomia);
        Assert.Equal(10, item.CodCondom);
        Assert.Equal("B1", item.CodBloco);
        Assert.Equal("101", item.CodEconom);
        Assert.Equal(2, response.PaginaAtual);
        Assert.Equal(10, response.ItensPorPagina);
        Assert.Equal(15, response.TotalItens);
        Assert.Equal(2, response.TotalPaginas);
    }

    [Fact]
    public async Task HandleAsync_SemResultados_RetornaListaVaziaSemErro()
    {
        var repositorio = new FakeUnidadeRepository
        {
            ResultadoListar = new PagedResult<Unidade>([], 1, 10, 0, 0),
        };

        var handler = new ListHandler(repositorio);

        var response = await handler.HandleAsync(
            new ListRequest(null, null, null, null, null, null),
            CancellationToken.None);

        Assert.Empty(response.Items);
        Assert.Equal(0, response.TotalItens);
        Assert.Equal(0, response.TotalPaginas);
    }
}
