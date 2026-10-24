using Gestion_Condominios.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gestion_Condominios.Data.Configurations
{
    public class UnidadConfiguration: IEntityTypeConfiguration<Unidad>
    {
        public void Configure(EntityTypeBuilder<Unidad> builder) {
            builder.ToTable("Unidades", t =>
            {
                t.HasCheckConstraint(
                    "CK_Unidades_Numero",
                    "LEN(LTRIM(RTRIM([NumeroUnidad]))) > 0");

                t.HasCheckConstraint(
                    "CK_Unidades_Tipo",
                    "LEN(LTRIM(RTRIM([Tipo]))) > 0");

                t.HasCheckConstraint(
                    "CK_Unidades_CantidadCuartos",
                    "[CantidadCuartos] >= 0");

                t.HasCheckConstraint(
                    "CK_Unidades_Superficie",
                    "[SuperficieM2] > 0");

                t.HasCheckConstraint(
                    "CK_Unidades_PagoMes",
                    "[PagoMes] > 0");
            }); 

            builder.HasKey(u => u.IdUnidad);


            builder.Property(u => u.IdUnidad)
                .ValueGeneratedOnAdd();
            builder.Property(u => u.NumeroUnidad)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(u => u.CantidadCuartos)
                .IsRequired();
            builder.Property(u => u.Tipo)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(u => u.SuperficieM2)
                .IsRequired()
                .HasPrecision(10, 2);
            builder.Property(u => u.PagoMes)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasIndex(u => new { u.IdCondominio, u.NumeroUnidad })
                .IsUnique()
                .HasDatabaseName(
                "UX_Unidades_Condominio_Numero");

            builder.HasOne(u => u.Condominio)
           .WithMany(c => c.Unidades)
           .HasForeignKey(u => u.IdCondominio)
           .IsRequired()
           .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
