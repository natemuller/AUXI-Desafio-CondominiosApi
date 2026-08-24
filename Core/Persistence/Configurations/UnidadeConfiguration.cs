using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations;

public sealed class UnidadeConfiguration
    : IEntityTypeConfiguration<Unidade>
{
    public void Configure(EntityTypeBuilder<Unidade> builder)
    {
        builder.ToTable("unidades", "public");

        builder.HasKey(x => x.Ideconomia);

        builder.Property(x => x.Ideconomia)
            .HasColumnName("ideconomia")
            .ValueGeneratedNever();

        builder.Property(x => x.CodCondom)
            .HasColumnName("codcondom");

        builder.Property(x => x.CodBloco)
            .HasColumnName("codbloco");

        builder.Property(x => x.CodEconom)
            .HasColumnName("codeconom");

        builder.Property(x => x.Fracao)
            .HasColumnName("fracao");

        builder.Property(x => x.Ativa)
            .HasColumnName("ativa");

        builder.Property(x => x.DataDesativa)
            .HasColumnName("datadesativa");

        builder.Property(x => x.DtAlteracao)
            .HasColumnName("dtalteracao");

        builder.Property(x => x.TipoUnidade)
            .HasColumnName("tipo_unidade");

        builder.Property(x => x.CodCondomino)
            .HasColumnName("cod_condomino");

        builder.Property(x => x.NomeCondomino)
            .HasColumnName("nome_condomino");

        builder.Property(x => x.EnderecoPrincipal)
            .HasColumnName("endereco_principal");

        builder.Property(x => x.EnderecoCorrespondencia)
            .HasColumnName("endereco_correspondencia");

        builder.Property(x => x.EnderecoCobranca)
            .HasColumnName("endereco_cobranca");

        builder.Property(x => x.CodPesDebConta)
            .HasColumnName("codpesdebconta");

        builder.Property(x => x.NomeDebConta)
            .HasColumnName("nome_debconta");

        builder.Property(x => x.CodFornec)
            .HasColumnName("codfornec");

        builder.Property(x => x.CodNaAdmDest)
            .HasColumnName("codnaadmdest");

        builder.Property(x => x.CodFornecEscrit)
            .HasColumnName("codfornecescrit");
    }
}
