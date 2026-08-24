namespace Core.Common;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int PaginaAtual,
    int ItensPorPagina,
    int TotalItens,
    int TotalPaginas
);
