namespace BlocosApi.Features.ListBlocos;

public sealed record ListRequest(
    int? CodCondom,
    string? CodBloco,
    string? Descricao,
    string? Ativo,
    int Pagina = 1);
