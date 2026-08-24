using BlocosApi.Features.GetBlocoByChave;
using BlocosApi.Tests.TestDoubles;
using Core.Models;

namespace BlocosApi.Tests.Features.GetBlocoByChave;

public class GetBlocoByChaveHandlerTests
{
    [Fact]
    public async Task HandleAsync_BlocoExistente_RetornaItemMapeado()
    {
        var repositorio = new FakeBlocoRepository
        {
            BlocoParaObter = new Bloco { CodCondom = 7, CodBloco = "A", Descricao = "Torre A" },
        };

        var handler = new GetBlocoByChaveHandler(repositorio);

        var response = await handler.HandleAsync(7, "A", CancellationToken.None);

        Assert.NotNull(response);
        Assert.Equal(7, response!.CodCondom);
        Assert.Equal("A", response.CodBloco);
        Assert.Equal("Torre A", response.Descricao);
        Assert.Equal((7, "A"), repositorio.UltimaChaveObtida);
    }

    [Fact]
    public async Task HandleAsync_BlocoInexistente_RetornaNull()
    {
        var repositorio = new FakeBlocoRepository { BlocoParaObter = null };
        var handler = new GetBlocoByChaveHandler(repositorio);

        var response = await handler.HandleAsync(999, "Z", CancellationToken.None);

        Assert.Null(response);
        Assert.Equal((999, "Z"), repositorio.UltimaChaveObtida);
    }
}
