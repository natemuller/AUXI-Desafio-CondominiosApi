namespace Core.Repositories.Condominios;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int PaginaAtual,
    int ItensPorPagina,
    int TotalItens,
    int TotalPaginas
);