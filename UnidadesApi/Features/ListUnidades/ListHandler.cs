using Core.Repositories.Unidades;

namespace UnidadesApi.Features.ListUnidades;

public sealed class ListHandler(IUnidadeRepository unidadeRepository)
{
    public async Task<ListResponse> HandleAsync(
        ListRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await unidadeRepository.ListarAsync(
            request.Pagina,
            request.CodCondom,
            request.CodBloco,
            request.CodEconom,
            request.TipoUnidade,
            request.Ativa,
            request.NomeCondomino,
            cancellationToken);

        var items = resultado.Items
            .Select(UnidadeItem.FromUnidade)
            .ToList();

        return new ListResponse(
            items,
            resultado.PaginaAtual,
            resultado.ItensPorPagina,
            resultado.TotalItens,
            resultado.TotalPaginas);
    }
}
