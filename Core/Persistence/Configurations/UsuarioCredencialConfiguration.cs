using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations;

public sealed class UsuarioCredencialConfiguration
    : IEntityTypeConfiguration<UsuarioCredencial>
{
    public void Configure(EntityTypeBuilder<UsuarioCredencial> builder)
    {
        builder.ToTable("usuario_credenciais", "public");

        builder.HasKey(x => x.UsuarioId);

        builder.Property(x => x.UsuarioId)
            .HasColumnName("usuario_id")
            .ValueGeneratedNever();

        builder.Property(x => x.SenhaHash)
            .HasColumnName("senha_hash");

        builder.Property(x => x.TentativasFalhas)
            .HasColumnName("tentativas_falhas");

        builder.Property(x => x.BloqueadoAte)
            .HasColumnName("bloqueado_ate");
    }
}
