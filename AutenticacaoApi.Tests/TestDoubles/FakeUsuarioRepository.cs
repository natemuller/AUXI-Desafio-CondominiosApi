using Core.Models;
using Core.Repositories.Usuarios;

namespace AutenticacaoApi.Tests.TestDoubles;

internal sealed class FakeUsuarioRepository : IUsuarioRepository
{
    public (Usuario Usuario, UsuarioCredencial Credencial)? ResultadoParaRetornar { get; set; }

    public string? UltimoCpfOuEmailRecebido { get; private set; }

    public Task<(Usuario Usuario, UsuarioCredencial Credencial)?> ObterPorCpfOuEmailAsync(
        string cpfOuEmail,
        CancellationToken cancellationToken = default)
    {
        UltimoCpfOuEmailRecebido = cpfOuEmail;

        return Task.FromResult(ResultadoParaRetornar);
    }
}
