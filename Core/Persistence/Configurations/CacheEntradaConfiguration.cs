using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations;

public sealed class CacheEntradaConfiguration
    : IEntityTypeConfiguration<CacheEntrada>
{
    public void Configure(EntityTypeBuilder<CacheEntrada> builder)
    {
        builder.ToTable("cache", "public");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.ChaveCache)
            .HasColumnName("chave_cache")
            .HasMaxLength(500);

        builder.Property(x => x.UrlDaConsulta)
            .HasColumnName("url_da_consulta")
            .HasMaxLength(1000);

        builder.Property(x => x.MetodoHttp)
            .HasColumnName("metodo_http")
            .HasMaxLength(10);

        builder.Property(x => x.TipoConsulta)
            .HasColumnName("tipo_consulta")
            .HasMaxLength(100);

        builder.Property(x => x.Entidade)
            .HasColumnName("entidade")
            .HasMaxLength(100);

        builder.Property(x => x.EntidadeId)
            .HasColumnName("entidade_id");

        builder.Property(x => x.Resposta)
            .HasColumnName("resposta")
            .HasColumnType("jsonb");

        builder.Property(x => x.StatusCode)
            .HasColumnName("status_code");

        builder.Property(x => x.CriadoEm)
            .HasColumnName("criado_em");

        builder.Property(x => x.ExpiradoEm)
            .HasColumnName("expirado_em");

        builder.Property(x => x.InvalidadoEm)
            .HasColumnName("invalidado_em");

        builder.Property(x => x.MotivoInvalidacao)
            .HasColumnName("motivo_invalidacao")
            .HasMaxLength(255);
    }
}
