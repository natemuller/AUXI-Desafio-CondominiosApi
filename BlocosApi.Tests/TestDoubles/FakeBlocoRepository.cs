using Core.Common;
using Core.Models;
using Core.Repositories.Blocos;

namespace BlocosApi.Tests.TestDoubles;

/// <summary>
/// Dublê manual de <see cref="IBlocoRepository"/> usado para testar os
/// handlers sem depender do EF Core / Postgres.
/// </summary>
internal sealed class FakeBlocoRepository : IBlocoRepository
{
    public PagedResult<Bloco> ResultadoListar { get; set; } =
        new([], 1, PaginationDefaults.ItensPorPagina, 0, 0);

    public Bloco? BlocoParaObter { get; set; }

    public (int Pagina, int? CodCondom, string? CodBloco, string? Descricao, string? Ativo)?
        UltimaChamadaListar
    {
        get; private set;
    }

    public (int CodCondom, string CodBloco)? UltimaChaveObtida { get; private set; }

    public Task<PagedResult<Bloco>> ListarAsync(
        int pagina,
        int? codCondom,
        string? codBloco,
        string? descricao,
        string? ativo,
        CancellationToken cancellationToken = default)
    {
        UltimaChamadaListar = (pagina, codCondom, codBloco, descricao, ativo);

        return Task.FromResult(ResultadoListar);
    }

    public Task<Bloco?> ObterPorChaveAsync(
        int codCondom,
        string codBloco,
        CancellationToken cancellationToken = default)
    {
        UltimaChaveObtida = (codCondom, codBloco);

        return Task.FromResult(BlocoParaObter);
    }
}
