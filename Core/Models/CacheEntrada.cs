namespace Core.Models;

public class CacheEntrada
{
    public Guid Id { get; set; }

    public string ChaveCache { get; set; } = string.Empty;

    public string UrlDaConsulta { get; set; } = string.Empty;

    public string MetodoHttp { get; set; } = string.Empty;

    public string TipoConsulta { get; set; } = string.Empty;

    public string Entidade { get; set; } = string.Empty;

    public int? EntidadeId { get; set; }

    /// <summary>
    /// Conteúdo JSON serializado da resposta em cache. Mapeado como texto no
    /// C# e persistido na coluna Postgres do tipo jsonb.
    /// </summary>
    public string Resposta { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public DateTimeOffset CriadoEm { get; set; }

    public DateTimeOffset ExpiradoEm { get; set; }

    public DateTimeOffset? InvalidadoEm { get; set; }

    public string? MotivoInvalidacao { get; set; }
}
