namespace Core.Caching;

public sealed class CacheOptions
{
    // ASSUNÇÃO: nenhum documento do desafio define um TTL de cache. 300
    // segundos (5 minutos) foi escolhido como um valor razoável padrão;
    // pode ser sobrescrito via configuração ("Cache:TtlSeconds").
    public int TtlSeconds { get; set; } = 300;
}
