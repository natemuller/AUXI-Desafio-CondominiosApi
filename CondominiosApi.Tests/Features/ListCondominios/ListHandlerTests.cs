using CondominiosApi.Features.ListCondominios;
using CondominiosApi.Tests.TestDoubles;
using Core.Common;
using Core.Models;

namespace CondominiosApi.Tests.Features.ListCondominios;

public class ListHandlerTests
{
    [Fact]
    public async Task HandleAsync_RepassaFiltrosParaORepositorio()
    {
        var repositorio = new FakeCondominioRepository();
        var handler = new ListHandler(repositorio);
        var request = new ListRequest(
            Cnpj: "12345678000199",
            CodCondom: 42,
            Nome: "Edifício",
            Pagina: 3);

        await handler.HandleAsync(request, CancellationToken.None);

        Assert.Equal(
            (3, "12345678000199", (int?)42, "Edifício"),
            repositorio.UltimaChamadaListar);
    }

    [Fact]
    public async Task HandleAsync_MapeiaItensEPaginacaoCorretamente()
    {
        var condominio = new Condominio
        {
            CodCondom = 10,
            NomeCondom = "Cond A",
            Cnpj = "111",
        };

        var repositorio = new FakeCondominioRepository
        {
            ResultadoListar = new PagedResult<Condominio>([condominio], 2, 10, 15, 2),
        };

        var handler = new ListHandler(repositorio);

        var response = await handler.HandleAsync(
            new ListRequest(null, null, null),
            CancellationToken.None);

        var item = Assert.Single(response.Items);
        Assert.Equal(10, item.CodCondom);
        Assert.Equal("Cond A", item.NomeCondom);
        Assert.Equal(2, response.PaginaAtual);
        Assert.Equal(10, response.ItensPorPagina);
        Assert.Equal(15, response.TotalItens);
        Assert.Equal(2, response.TotalPaginas);
    }

    [Fact]
    public async Task HandleAsync_SemResultados_RetornaListaVaziaSemErro()
    {
        var repositorio = new FakeCondominioRepository
        {
            ResultadoListar = new PagedResult<Condominio>([], 1, 10, 0, 0),
        };

        var handler = new ListHandler(repositorio);

        var response = await handler.HandleAsync(
            new ListRequest(null, null, null),
            CancellationToken.None);

        Assert.Empty(response.Items);
        Assert.Equal(0, response.TotalItens);
        Assert.Equal(0, response.TotalPaginas);
    }
}
