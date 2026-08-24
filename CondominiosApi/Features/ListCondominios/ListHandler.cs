using Core.Repositories.Condominios;

namespace CondominiosApi.Features.ListCondominios;

public sealed class ListHandler(ICondominioRepository condominioRepository)
{
    public async Task<ListResponse> HandleAsync(
        ListRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await condominioRepository.ListarAsync(
            request.Pagina,
            request.Cnpj,
            request.CodCondom,
            request.Nome,
            cancellationToken);

        var items = resultado.Items
            .Select(CondominioItem.FromCondominio)
            .ToList();

        return new ListResponse(
            items,
            resultado.PaginaAtual,
            resultado.ItensPorPagina,
            resultado.TotalItens,
            resultado.TotalPaginas);
    }
}
