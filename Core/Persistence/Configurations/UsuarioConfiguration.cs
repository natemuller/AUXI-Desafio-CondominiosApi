using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations;

public sealed class UsuarioConfiguration
    : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios", "public");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Cpf)
            .HasColumnName("cpf")
            .HasMaxLength(11)
            .IsFixedLength();

        builder.Property(x => x.NomeCompleto)
            .HasColumnName("nome_completo")
            .HasMaxLength(200);

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(254);

        builder.Property(x => x.Telefone)
            .HasColumnName("telefone")
            .HasMaxLength(15);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(20);

        builder.Property(x => x.CriadoEm)
            .HasColumnName("criado_em");

        builder.Property(x => x.AtualizadoEm)
            .HasColumnName("atualizado_em");
    }
}
