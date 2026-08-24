using Core.Common;
using Core.Models;

namespace Core.Repositories.Unidades;

public interface IUnidadeRepository
{
    Task<PagedResult<Unidade>> ListarAsync(
        int pagina,
        int? codCondom,
        string? codBloco,
        string? codEconom,
        string? tipoUnidade,
        string? ativa,
        string? nomeCondomino,
        CancellationToken cancellationToken = default);

    Task<Unidade?> ObterPorIdAsync(
        int ideconomia,
        CancellationToken cancellationToken = default);
}
