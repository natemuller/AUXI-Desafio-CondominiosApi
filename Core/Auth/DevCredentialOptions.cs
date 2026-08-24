namespace Core.Auth;

/// <summary>
/// Atalho de credencial de desenvolvimento, usado apenas pela AutenticacaoApi
/// para permitir login sem depender de hash de senha real durante o
/// desenvolvimento/demonstração do desafio. NÃO deve ser habilitado em
/// produção.
/// </summary>
public sealed class DevCredentialOptions
{
    public bool Enabled { get; set; }

    public string Cpf { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
