using Core.Common;
using Core.Models;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories.Blocos;

public sealed class BlocoRepository(AuxiDbContext dbContext)
    : IBlocoRepository
{
    public async Task<PagedResult<Bloco>> ListarAsync(
        int pagina,
        int? codCondom,
        string? codBloco,
        string? descricao,
        string? ativo,
        CancellationToken cancellationToken = default)
    {
        pagina = pagina < 1 ? 1 : pagina;

        IQueryable<Bloco> query = dbContext.Blocos
            .AsNoTracking();

        if (codCondom.HasValue)
        {
            query = query.Where(bloco =>
                bloco.CodCondom == codCondom.Value);
        }

        if (!string.IsNullOrWhiteSpace(codBloco))
        {
            var filtro = codBloco.Trim();

            query = query.Where(bloco =>
                bloco.CodBloco == filtro);
        }

        if (!string.IsNullOrWhiteSpace(descricao))
        {
            var filtro = descricao.Trim();

            query = query.Where(bloco =>
                bloco.Descricao != null &&
                EF.Functions.ILike(
                    bloco.Descricao,
                    $"%{filtro}%"));
        }

        if (!string.IsNullOrWhiteSpace(ativo))
        {
            var filtro = ativo.Trim();

            query = query.Where(bloco =>
                bloco.Ativo == filtro);
        }

        var totalItens = await query.CountAsync(cancellationToken);

        var blocos = await query
            .OrderBy(bloco => bloco.CodCondom)
            .ThenBy(bloco => bloco.CodBloco)
            .Skip((pagina - 1) * PaginationDefaults.ItensPorPagina)
            .Take(PaginationDefaults.ItensPorPagina)
            .ToListAsync(cancellationToken);

        var totalPaginas = (int)Math.Ceiling(
            totalItens / (double)PaginationDefaults.ItensPorPagina);

        return new PagedResult<Bloco>(
            blocos,
            pagina,
            PaginationDefaults.ItensPorPagina,
            totalItens,
            totalPaginas);
    }

    public Task<Bloco?> ObterPorChaveAsync(
        int codCondom,
        string codBloco,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Blocos
            .AsNoTracking()
            .FirstOrDefaultAsync(
                bloco =>
                    bloco.CodCondom == codCondom &&
                    bloco.CodBloco == codBloco,
                cancellationToken);
    }
}
