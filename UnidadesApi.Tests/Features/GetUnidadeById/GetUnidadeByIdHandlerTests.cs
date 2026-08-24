using Core.Models;
using UnidadesApi.Features.GetUnidadeById;
using UnidadesApi.Tests.TestDoubles;

namespace UnidadesApi.Tests.Features.GetUnidadeById;

public class GetUnidadeByIdHandlerTests
{
    [Fact]
    public async Task HandleAsync_UnidadeExistente_RetornaItemMapeado()
    {
        var repositorio = new FakeUnidadeRepository
        {
            UnidadeParaObter = new Unidade
            {
                Ideconomia = 7,
                CodCondom = 1,
                CodBloco = "A",
                CodEconom = "101",
            },
        };

        var handler = new GetUnidadeByIdHandler(repositorio);

        var response = await handler.HandleAsync(7, CancellationToken.None);

        Assert.NotNull(response);
        Assert.Equal(7, response!.Ideconomia);
        Assert.Equal("101", response.CodEconom);
        Assert.Equal(7, repositorio.UltimoIdObtido);
    }

    [Fact]
    public async Task HandleAsync_UnidadeInexistente_RetornaNull()
    {
        var repositorio = new FakeUnidadeRepository { UnidadeParaObter = null };
        var handler = new GetUnidadeByIdHandler(repositorio);

        var response = await handler.HandleAsync(999, CancellationToken.None);

        Assert.Null(response);
        Assert.Equal(999, repositorio.UltimoIdObtido);
    }
}
