using CondominiosApi.Features.ListCondominios;
using Core.Repositories.Condominios;

namespace CondominiosApi.Features.GetByCodCondom;

public sealed class GetByCodCondomHandler(ICondominioRepository condominioRepository)
{
    public async Task<GetByCodCondomResponse?> HandleAsync(
        int codCondom,
        CancellationToken cancellationToken)
    {
        var condominio = await condominioRepository.ObterPorCodigoAsync(
            codCondom,
            cancellationToken);

        return condominio is null
            ? null
            : CondominioItem.FromCondominio(condominio);
    }
}
