namespace UnidadesApi.Features.ListUnidades;

public sealed record ListRequest(
    int? CodCondom,
    string? CodBloco,
    string? CodEconom,
    string? TipoUnidade,
    string? Ativa,
    string? NomeCondomino,
    int Pagina = 1);
