using Microsoft.EntityFrameworkCore;

namespace Core.Repositories.Condominios;

public sealed class CondominioRepository(AuxiDbContext dbContext)
    : ICondominioRepository
{
    private const int ItensPorPagina = 10;

    public async Task<PagedResult<Condominio>> ListarAsync(
        int pagina,
        string? nome,
        CancellationToken cancellationToken = default)
    {
        pagina = pagina < 1 ? 1 : pagina;

        IQueryable<Condominio> query = dbContext.Condominios
            .AsNoTracking();

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
            .Skip((pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync(cancellationToken);

        var totalPaginas = (int)Math.Ceiling(
            totalItens / (double)ItensPorPagina);

        return new PagedResult<Condominio>(
            condominios,
            pagina,
            ItensPorPagina,
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