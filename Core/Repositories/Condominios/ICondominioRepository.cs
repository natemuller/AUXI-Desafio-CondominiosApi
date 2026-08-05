namespace Core.Repositories.Condominios;

public interface ICondominioRepository
{
    Task<PagedResult<Condominio>> ListarAsync(
        int pagina,
        string? nome,
        CancellationToken cancellationToken = default);

    Task<Condominio?> ObterPorCodigoAsync(
        int codCondom,
        CancellationToken cancellationToken = default);
}