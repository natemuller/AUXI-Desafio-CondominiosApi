namespace Core.Models;

public class Usuario
{
    public Guid Id { get; set; }

    public string Cpf { get; set; } = string.Empty;

    public string NomeCompleto { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Telefone { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTimeOffset CriadoEm { get; set; }

    public DateTimeOffset AtualizadoEm { get; set; }
}
