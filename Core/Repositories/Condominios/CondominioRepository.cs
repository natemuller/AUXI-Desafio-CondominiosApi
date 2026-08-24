using Core.Common;
using Core.Models;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories.Condominios;

public sealed class CondominioRepository(AuxiDbContext dbContext)
    : ICondominioRepository
{
    public async Task<PagedResult<Condominio>> ListarAsync(
        int pagina,
        string? cnpj,
        int? codCondom,
        string? nome,
        CancellationToken cancellationToken = default)
    {
        pagina = pagina < 1 ? 1 : pagina;

        IQueryable<Condominio> query = dbContext.Condominios
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(cnpj))
        {
            var filtro = cnpj.Trim();

            query = query.Where(condominio =>
                condominio.Cnpj == filtro);
        }

        if (codCondom.HasValue)
        {
            query = query.Where(condominio =>
                condominio.CodCondom == codCondom.Value);
        }

        if (!string.IsNullOrWhiteSpace(nome))
        {
            var filtro = nome.Trim();

            query = query.Where(condominio =>
                condominio.NomeCondom != null &&
                EF.Functions.ILike(
                    condominio.NomeCondom,
                    $"%{filtro}%"));
        }

        var totalItens = await query.CountAsync(cancellationToken);

        var condominios = await query
            .OrderBy(condominio => condominio.NomeCondom)
            .ThenBy(condominio => condominio.CodCondom)
            .Skip((pagina - 1) * PaginationDefaults.ItensPorPagina)
            .Take(PaginationDefaults.ItensPorPagina)
            .ToListAsync(cancellationToken);

        var totalPaginas = (int)Math.Ceiling(
            totalItens / (double)PaginationDefaults.ItensPorPagina);

        return new PagedResult<Condominio>(
            condominios,
            pagina,
            PaginationDefaults.ItensPorPagina,
            totalItens,
            totalPaginas);
    }

    public Task<Condominio?> ObterPorCodigoAsync(
        int codCondom,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Condominios
            .AsNoTracking()
            .FirstOrDefaultAsync(
                condominio => condominio.CodCondom == codCondom,
                cancellationToken);
    }
}
