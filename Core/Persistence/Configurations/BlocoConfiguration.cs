using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations;

public sealed class BlocoConfiguration
    : IEntityTypeConfiguration<Bloco>
{
    public void Configure(EntityTypeBuilder<Bloco> builder)
    {
        builder.ToTable("blocos", "public");

        builder.HasKey(x => new { x.CodCondom, x.CodBloco });

        builder.Property(x => x.CodCondom)
            .HasColumnName("codcondom");

        builder.Property(x => x.CodBloco)
            .HasColumnName("codbloco");

        builder.Property(x => x.CodBlocoBase)
            .HasColumnName("codblocobase");

        builder.Property(x => x.Descricao)
            .HasColumnName("descricao");

        builder.Property(x => x.QtdEconomias)
            .HasColumnName("qtdeconomias");

        builder.Property(x => x.TipoLograd)
            .HasColumnName("tipolograd");

        builder.Property(x => x.Lograd)
            .HasColumnName("lograd");

        builder.Property(x => x.Numero)
            .HasColumnName("numero");

        builder.Property(x => x.Bairro)
            .HasColumnName("bairro");

        builder.Property(x => x.Cidade)
            .HasColumnName("cidade");

        builder.Property(x => x.Uf)
            .HasColumnName("uf");

        builder.Property(x => x.Cep8Log)
            .HasColumnName("cep8_log");

        builder.Property(x => x.Ativo)
            .HasColumnName("ativo");

        builder.Property(x => x.TipoBloco)
            .HasColumnName("tipo_bloco");
    }
}
