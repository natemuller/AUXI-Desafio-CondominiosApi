using Core.Models;
using Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories.Usuarios;

public sealed class UsuarioRepository(AuxiDbContext dbContext)
    : IUsuarioRepository
{
    public async Task<(Usuario Usuario, UsuarioCredencial Credencial)?> ObterPorCpfOuEmailAsync(
        string cpfOuEmail,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpfOuEmail))
        {
            return null;
        }

        var somenteDigitos = new string(
            cpfOuEmail.Where(char.IsDigit).ToArray());

        Usuario? usuario = null;

        if (somenteDigitos.Length == 11)
        {
            usuario = await dbContext.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Cpf == somenteDigitos,
                    cancellationToken);
        }

        if (usuario is null)
        {
            var email = cpfOuEmail.Trim();

            usuario = await dbContext.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => EF.Functions.ILike(x.Email, email),
                    cancellationToken);
        }

        if (usuario is null)
        {
            return null;
        }

        var credencial = await dbContext.UsuarioCredenciais
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UsuarioId == usuario.Id,
                cancellationToken);

        if (credencial is null)
        {
            return null;
        }

        return (usuario, credencial);
    }
}
