using Core.Common;
using Core.Models;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories.Unidades;

public sealed class UnidadeRepository(AuxiDbContext dbContext)
    : IUnidadeRepository
{
    public async Task<PagedResult<Unidade>> ListarAsync(
        int pagina,
        int? codCondom,
        string? codBloco,
        string? codEconom,
        string? tipoUnidade,
        string? ativa,
        string? nomeCondomino,
        CancellationToken cancellationToken = default)
    {
        pagina = pagina < 1 ? 1 : pagina;

        IQueryable<Unidade> query = dbContext.Unidades
            .AsNoTracking();

        if (codCondom.HasValue)
        {
            query = query.Where(unidade =>
                unidade.CodCondom == codCondom.Value);
        }

        if (!string.IsNullOrWhiteSpace(codBloco))
        {
            var filtro = codBloco.Trim();

            query = query.Where(unidade =>
                unidade.CodBloco == filtro);
        }

        if (!string.IsNullOrWhiteSpace(codEconom))
        {
            var filtro = codEconom.Trim();

            query = query.Where(unidade =>
                unidade.CodEconom == filtro);
        }

        if (!string.IsNullOrWhiteSpace(tipoUnidade))
        {
            var filtro = tipoUnidade.Trim();

            query = query.Where(unidade =>
                unidade.TipoUnidade == filtro);
        }

        if (!string.IsNullOrWhiteSpace(ativa))
        {
            var filtro = ativa.Trim();

            query = query.Where(unidade =>
                unidade.Ativa == filtro);
        }

        if (!string.IsNullOrWhiteSpace(nomeCondomino))
        {
            var filtro = nomeCondomino.Trim();

            query = query.Where(unidade =>
                unidade.NomeCondomino != null &&
                EF.Functions.ILike(
                    unidade.NomeCondomino,
                    $"%{filtro}%"));
        }

        var totalItens = await query.CountAsync(cancellationToken);

        var unidades = await query
            .OrderBy(unidade => unidade.CodCondom)
            .ThenBy(unidade => unidade.CodBloco)
            .ThenBy(unidade => unidade.Ideconomia)
            .Skip((pagina - 1) * PaginationDefaults.ItensPorPagina)
            .Take(PaginationDefaults.ItensPorPagina)
            .ToListAsync(cancellationToken);

        var totalPaginas = (int)Math.Ceiling(
            totalItens / (double)PaginationDefaults.ItensPorPagina);

        return new PagedResult<Unidade>(
            unidades,
            pagina,
            PaginationDefaults.ItensPorPagina,
            totalItens,
            totalPaginas);
    }

    public Task<Unidade?> ObterPorIdAsync(
        int ideconomia,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Unidades
            .AsNoTracking()
            .FirstOrDefaultAsync(
                unidade => unidade.Ideconomia == ideconomia,
                cancellationToken);
    }
}
