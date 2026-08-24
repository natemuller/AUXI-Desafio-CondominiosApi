using Core.Repositories.Blocos;

namespace BlocosApi.Features.ListBlocos;

public sealed class ListHandler(IBlocoRepository blocoRepository)
{
    public async Task<ListResponse> HandleAsync(
        ListRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await blocoRepository.ListarAsync(
            request.Pagina,
            request.CodCondom,
            request.CodBloco,
            request.Descricao,
            request.Ativo,
            cancellationToken);

        var items = resultado.Items
            .Select(BlocoItem.FromBloco)
            .ToList();

        return new ListResponse(
            items,
            resultado.PaginaAtual,
            resultado.ItensPorPagina,
            resultado.TotalItens,
            resultado.TotalPaginas);
    }
}
