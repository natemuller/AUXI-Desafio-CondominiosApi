using Core.Common;
using Core.Models;

namespace Core.Repositories.Blocos;

public interface IBlocoRepository
{
    Task<PagedResult<Bloco>> ListarAsync(
        int pagina,
        int? codCondom,
        string? codBloco,
        string? descricao,
        string? ativo,
        CancellationToken cancellationToken = default);

    Task<Bloco?> ObterPorChaveAsync(
        int codCondom,
        string codBloco,
        CancellationToken cancellationToken = default);
}
