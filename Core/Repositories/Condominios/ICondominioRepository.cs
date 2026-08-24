using Core.Common;
using Core.Models;

namespace Core.Repositories.Condominios;

public interface ICondominioRepository
{
    Task<PagedResult<Condominio>> ListarAsync(
        int pagina,
        string? cnpj,
        int? codCondom,
        string? nome,
        CancellationToken cancellationToken = default);

    Task<Condominio?> ObterPorCodigoAsync(
        int codCondom,
        CancellationToken cancellationToken = default);
}
