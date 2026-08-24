using Core.Repositories.Unidades;
using UnidadesApi.Features.ListUnidades;

namespace UnidadesApi.Features.GetUnidadeById;

public sealed class GetUnidadeByIdHandler(IUnidadeRepository unidadeRepository)
{
    public async Task<GetUnidadeByIdResponse?> HandleAsync(
        int ideconomia,
        CancellationToken cancellationToken)
    {
        var unidade = await unidadeRepository.ObterPorIdAsync(
            ideconomia,
            cancellationToken);

        return unidade is null
            ? null
            : UnidadeItem.FromUnidade(unidade);
    }
}
