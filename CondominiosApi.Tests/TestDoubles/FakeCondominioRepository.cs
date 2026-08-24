using Core.Common;
using Core.Models;
using Core.Repositories.Condominios;

namespace CondominiosApi.Tests.TestDoubles;

/// <summary>
/// Dublê manual de <see cref="ICondominioRepository"/> usado para testar os
/// handlers sem depender do EF Core / Postgres.
/// </summary>
internal sealed class FakeCondominioRepository : ICondominioRepository
{
    public PagedResult<Condominio> ResultadoListar { get; set; } =
        new([], 1, PaginationDefaults.ItensPorPagina, 0, 0);

    public Condominio? CondominioParaObter { get; set; }

    public (int Pagina, string? Cnpj, int? CodCondom, string? Nome)? UltimaChamadaListar
    {
        get; private set;
    }

    public int? UltimoCodigoObtido { get; private set; }

    public Task<PagedResult<Condominio>> ListarAsync(
        int pagina,
        string? cnpj,
        int? codCondom,
        string? nome,
        CancellationToken cancellationToken = default)
    {
        UltimaChamadaListar = (pagina, cnpj, codCondom, nome);

        return Task.FromResult(ResultadoListar);
    }

    public Task<Condominio?> ObterPorCodigoAsync(
        int codCondom,
        CancellationToken cancellationToken = default)
    {
        UltimoCodigoObtido = codCondom;

        return Task.FromResult(CondominioParaObter);
    }
}
