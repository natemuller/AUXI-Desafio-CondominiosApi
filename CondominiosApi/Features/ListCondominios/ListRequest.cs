namespace CondominiosApi.Features.ListCondominios;

public sealed record ListRequest(
    string? Cnpj,
    int? CodCondom,
    string? Nome,
    int Pagina = 1);
