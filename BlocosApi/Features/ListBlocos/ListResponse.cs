using Core.Models;

namespace BlocosApi.Features.ListBlocos;

public sealed record ListResponse(
    IReadOnlyCollection<BlocoItem> Items,
    int PaginaAtual,
    int ItensPorPagina,
    int TotalItens,
    int TotalPaginas);

public sealed record BlocoItem(
    int CodCondom,
    string CodBloco,
    string? CodBlocoBase,
    string? Descricao,
    int? QtdEconomias,
    string? TipoLograd,
    string? Lograd,
    string? Numero,
    string? Bairro,
    string? Cidade,
    string? Uf,
    string? Cep8Log,
    string? Ativo,
    string? TipoBloco)
{
    public static BlocoItem FromBloco(Bloco bloco) => new(
        bloco.CodCondom,
        bloco.CodBloco,
        bloco.CodBlocoBase,
        bloco.Descricao,
        bloco.QtdEconomias,
        bloco.TipoLograd,
        bloco.Lograd,
        bloco.Numero,
        bloco.Bairro,
        bloco.Cidade,
        bloco.Uf,
        bloco.Cep8Log,
        bloco.Ativo,
        bloco.TipoBloco);
}
