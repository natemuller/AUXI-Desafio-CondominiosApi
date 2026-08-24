using BlocosApi.Features.ListBlocos;
using Core.Repositories.Blocos;

namespace BlocosApi.Features.GetBlocoByChave;

public sealed class GetBlocoByChaveHandler(IBlocoRepository blocoRepository)
{
    public async Task<GetBlocoByChaveResponse?> HandleAsync(
        int codCondom,
        string codBloco,
        CancellationToken cancellationToken)
    {
        var bloco = await blocoRepository.ObterPorChaveAsync(
            codCondom,
            codBloco,
            cancellationToken);

        return bloco is null
            ? null
            : BlocoItem.FromBloco(bloco);
    }
}
