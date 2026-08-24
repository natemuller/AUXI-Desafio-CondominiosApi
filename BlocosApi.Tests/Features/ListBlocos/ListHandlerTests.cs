using BlocosApi.Features.ListBlocos;
using BlocosApi.Tests.TestDoubles;
using Core.Common;
using Core.Models;

namespace BlocosApi.Tests.Features.ListBlocos;

public class ListHandlerTests
{
    [Fact]
    public async Task HandleAsync_RepassaFiltrosParaORepositorio()
    {
        var repositorio = new FakeBlocoRepository();
        var handler = new ListHandler(repositorio);
        var request = new ListRequest(
            CodCondom: 42,
            CodBloco: "A",
            Descricao: "Torre A",
            Ativo: "S",
            Pagina: 2);

        await handler.HandleAsync(request, CancellationToken.None);

        Assert.Equal(
            (2, (int?)42, "A", "Torre A", "S"),
            repositorio.UltimaChamadaListar);
    }

    [Fact]
    public async Task HandleAsync_MapeiaItensEPaginacaoCorretamente()
    {
        var bloco = new Bloco
        {
            CodCondom = 10,
            CodBloco = "B1",
            Descricao = "Bloco 1",
        };

        var repositorio = new FakeBlocoRepository
        {
            ResultadoListar = new PagedResult<Bloco>([bloco], 2, 10, 15, 2),
        };

        var handler = new ListHandler(repositorio);

        var response = await handler.HandleAsync(
            new ListRequest(null, null, null, null),
            CancellationToken.None);

        var item = Assert.Single(response.Items);
        Assert.Equal(10, item.CodCondom);
        Assert.Equal("B1", item.CodBloco);
        Assert.Equal("Bloco 1", item.Descricao);
        Assert.Equal(2, response.PaginaAtual);
        Assert.Equal(10, response.ItensPorPagina);
        Assert.Equal(15, response.TotalItens);
        Assert.Equal(2, response.TotalPaginas);
    }

    [Fact]
    public async Task HandleAsync_SemResultados_RetornaListaVaziaSemErro()
    {
        var repositorio = new FakeBlocoRepository
        {
            ResultadoListar = new PagedResult<Bloco>([], 1, 10, 0, 0),
        };

        var handler = new ListHandler(repositorio);

        var response = await handler.HandleAsync(
            new ListRequest(null, null, null, null),
            CancellationToken.None);

        Assert.Empty(response.Items);
        Assert.Equal(0, response.TotalItens);
        Assert.Equal(0, response.TotalPaginas);
    }
}
