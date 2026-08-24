using Core.Models;

namespace Core.Repositories.Usuarios;

public interface IUsuarioRepository
{
    Task<(Usuario Usuario, UsuarioCredencial Credencial)?> ObterPorCpfOuEmailAsync(
        string cpfOuEmail,
        CancellationToken cancellationToken = default);
}
