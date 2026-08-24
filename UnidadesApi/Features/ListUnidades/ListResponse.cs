using Core.Models;

namespace UnidadesApi.Features.ListUnidades;

public sealed record ListResponse(
    IReadOnlyCollection<UnidadeItem> Items,
    int PaginaAtual,
    int ItensPorPagina,
    int TotalItens,
    int TotalPaginas);

public sealed record UnidadeItem(
    int Ideconomia,
    int CodCondom,
    string CodBloco,
    string? CodEconom,
    decimal? Fracao,
    string? Ativa,
    decimal? DataDesativa,
    DateTime? DtAlteracao,
    string? TipoUnidade,
    string? CodCondomino,
    string? NomeCondomino,
    string? EnderecoPrincipal,
    string? EnderecoCorrespondencia,
    string? EnderecoCobranca,
    string? CodPesDebConta,
    string? NomeDebConta,
    string? CodFornec,
    string? CodNaAdmDest,
    string? CodFornecEscrit)
{
    public static UnidadeItem FromUnidade(Unidade unidade) => new(
        unidade.Ideconomia,
        unidade.CodCondom,
        unidade.CodBloco,
        unidade.CodEconom,
        unidade.Fracao,
        unidade.Ativa,
        unidade.DataDesativa,
        unidade.DtAlteracao,
        unidade.TipoUnidade,
        unidade.CodCondomino,
        unidade.NomeCondomino,
        unidade.EnderecoPrincipal,
        unidade.EnderecoCorrespondencia,
        unidade.EnderecoCobranca,
        unidade.CodPesDebConta,
        unidade.NomeDebConta,
        unidade.CodFornec,
        unidade.CodNaAdmDest,
        unidade.CodFornecEscrit);
}
