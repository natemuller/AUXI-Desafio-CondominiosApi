using Core.Common;
using Core.Models;
using Core.Repositories.Unidades;

namespace UnidadesApi.Tests.TestDoubles;

/// <summary>
/// Dublê manual de <see cref="IUnidadeRepository"/> usado para testar os
/// handlers sem depender do EF Core / Postgres.
/// </summary>
internal sealed class FakeUnidadeRepository : IUnidadeRepository
{
    public PagedResult<Unidade> ResultadoListar { get; set; } =
        new([], 1, PaginationDefaults.ItensPorPagina, 0, 0);

    public Unidade? UnidadeParaObter { get; set; }

    public (int Pagina, int? CodCondom, string? CodBloco, string? CodEconom,
        string? TipoUnidade, string? Ativa, string? NomeCondomino)? UltimaChamadaListar
    {
        get; private set;
    }

    public int? UltimoIdObtido { get; private set; }

    public Task<PagedResult<Unidade>> ListarAsync(
        int pagina,
        int? codCondom,
        string? codBloco,
        string? codEconom,
        string? tipoUnidade,
        string? ativa,
        string? nomeCondomino,
        CancellationToken cancellationToken = default)
    {
        UltimaChamadaListar =
            (pagina, codCondom, codBloco, codEconom, tipoUnidade, ativa, nomeCondomino);

        return Task.FromResult(ResultadoListar);
    }

    public Task<Unidade?> ObterPorIdAsync(
        int ideconomia,
        CancellationToken cancellationToken = default)
    {
        UltimoIdObtido = ideconomia;

        return Task.FromResult(UnidadeParaObter);
    }
}
