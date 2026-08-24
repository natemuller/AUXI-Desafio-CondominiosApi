namespace Core.Models;

public class UsuarioCredencial
{
    public Guid UsuarioId { get; set; }

    public string SenhaHash { get; set; } = string.Empty;

    public short TentativasFalhas { get; set; }

    public DateTimeOffset? BloqueadoAte { get; set; }
}
