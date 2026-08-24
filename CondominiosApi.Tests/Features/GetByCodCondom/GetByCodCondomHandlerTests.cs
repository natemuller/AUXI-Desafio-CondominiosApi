using CondominiosApi.Features.GetByCodCondom;
using CondominiosApi.Tests.TestDoubles;
using Core.Models;

namespace CondominiosApi.Tests.Features.GetByCodCondom;

public class GetByCodCondomHandlerTests
{
    [Fact]
    public async Task HandleAsync_CondominioExistente_RetornaItemMapeado()
    {
        var repositorio = new FakeCondominioRepository
        {
            CondominioParaObter = new Condominio { CodCondom = 7, NomeCondom = "Cond Teste" },
        };

        var handler = new GetByCodCondomHandler(repositorio);

        var response = await handler.HandleAsync(7, CancellationToken.None);

        Assert.NotNull(response);
        Assert.Equal(7, response!.CodCondom);
        Assert.Equal("Cond Teste", response.NomeCondom);
        Assert.Equal(7, repositorio.UltimoCodigoObtido);
    }

    [Fact]
    public async Task HandleAsync_CondominioInexistente_RetornaNull()
    {
        var repositorio = new FakeCondominioRepository { CondominioParaObter = null };
        var handler = new GetByCodCondomHandler(repositorio);

        var response = await handler.HandleAsync(999, CancellationToken.None);

        Assert.Null(response);
        Assert.Equal(999, repositorio.UltimoCodigoObtido);
    }
}
