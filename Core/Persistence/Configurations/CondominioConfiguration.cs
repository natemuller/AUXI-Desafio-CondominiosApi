using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations;

public sealed class CondominioConfiguration
    : IEntityTypeConfiguration<Condominio>
{
    public void Configure(EntityTypeBuilder<Condominio> builder)
    {
        builder.ToTable("condominios", "public");

        builder.HasKey(x => x.CodCondom);

        builder.Property(x => x.CodCondom)
            .HasColumnName("codcondom")
            .ValueGeneratedNever();

        builder.Property(x => x.NomeCondom)
            .HasColumnName("nomecondom");

        builder.Property(x => x.Ativo)
            .HasColumnName("ativo");

        builder.Property(x => x.Cnpj)
            .HasColumnName("cnpj");

        builder.Property(x => x.Cei)
            .HasColumnName("cei");

        builder.Property(x => x.InscrMunicip)
            .HasColumnName("inscrmunicip");

        builder.Property(x => x.QtdBlocos)
            .HasColumnName("qtdblocos");

        builder.Property(x => x.QtdUnidades)
            .HasColumnName("qtdunidades");

        builder.Property(x => x.TotalFracao)
            .HasColumnName("totalfracao");

        builder.Property(x => x.DiaVencDoc)
            .HasColumnName("diavencdoc");

        builder.Property(x => x.DataInicioAdm)
            .HasColumnName("datainicioadm");

        builder.Property(x => x.DataDistrato)
            .HasColumnName("datadistrato");

        builder.Property(x => x.MotivoDistrato)
            .HasColumnName("motivodistrato");

        builder.Property(x => x.Assessor)
            .HasColumnName("assessor");

        builder.Property(x => x.Filial)
            .HasColumnName("filial");

        builder.Property(x => x.Agencia)
            .HasColumnName("agencia");

        builder.Property(x => x.Sindico)
            .HasColumnName("sindico");

        builder.Property(x => x.SubSindico)
            .HasColumnName("subsindico");

        builder.Property(x => x.Conselheiro)
            .HasColumnName("conselheiro");

        builder.Property(x => x.Gestor)
            .HasColumnName("gestor");

        builder.Property(x => x.ConselhoFiscal)
            .HasColumnName("conselhofiscal");

        builder.Property(x => x.ConselhoConsultivo)
            .HasColumnName("conselhoconsultivo");

        builder.Property(x => x.ConselhoSuplente)
            .HasColumnName("conselhosuplente");

        builder.Property(x => x.TipoCondominio)
            .HasColumnName("tipocondominio");

        builder.Property(x => x.TipoCategoria)
            .HasColumnName("tipocategoria");

        builder.Property(x => x.DtAlteracao)
            .HasColumnName("dtalteracao");

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

        builder.Property(x => x.Cep8Log)
            .HasColumnName("cep8log");

        builder.Property(x => x.Uf)
            .HasColumnName("uf");

        builder.Property(x => x.CodPessoaSindico)
            .HasColumnName("codpessoasindico");

        builder.Property(x => x.NomeSindico)
            .HasColumnName("nomesindico");

        builder.Property(x => x.CpfDocnpj)
            .HasColumnName("cpfdocnpj");

        builder.Property(x => x.CondGarantido)
            .HasColumnName("condgarantido");

        builder.Property(x => x.TipoConta)
            .HasColumnName("tipoconta");

        builder.Property(x => x.ObsCobranca)
            .HasColumnName("obscobranca");

        builder.Property(x => x.Garantidora)
            .HasColumnName("garantidora");
    }
}