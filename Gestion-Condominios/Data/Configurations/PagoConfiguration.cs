using Gestion_Condominios.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gestion_Condominios.Data.Configurations
{
    public class PagoConfiguration: IEntityTypeConfiguration<Pago>
    {
        public void Configure(EntityTypeBuilder<Pago> builder)
        {
            builder.ToTable("Pagos", t =>
            {
                t.HasCheckConstraint(
               "CK_Pagos_Monto",
               "[Monto] > 0");

                t.HasCheckConstraint(
                    "CK_Pagos_Anio",
                    "[Anio] BETWEEN 1 AND 9999");

                t.HasCheckConstraint(
                    "CK_Pagos_Mes",
                    "[Mes] BETWEEN 1 AND 12");

                t.HasCheckConstraint(
                    "CK_Pagos_Concepto",
                    "LEN(LTRIM(RTRIM([Concepto]))) > 0");

                t.HasCheckConstraint(
                    "CK_Pagos_EstadoFecha",
                    @"(
                    ([Estado] = 'PENDIENTE'
                     AND [FechaPago] IS NULL)
                    OR
                    ([Estado] = 'PAGADO'
                     AND [FechaPago] IS NOT NULL)
                  )");
            });
            builder.HasKey(c => c.IdPago);

            builder.Property(p => p.IdPago)
                .ValueGeneratedOnAdd();


            builder.Property(p => p.Monto)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.Anio)
                .IsRequired();

            builder.Property(p => p.Mes)
                .IsRequired();

            builder.Property(p => p.FechaVencimiento)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(p => p.FechaPago)
                .IsRequired(false)
                .HasColumnType("date");

            builder.Property(p => p.Estado)
                .IsRequired()
                .HasMaxLength(12)
                .HasDefaultValue("PENDIENTE");

            builder.Property(p => p.Concepto)
                .IsRequired()
                .HasMaxLength(200);


            builder.HasIndex(p => new
            {
                p.IdUnidad,
                p.Anio,
                p.Mes
            })
           .IsUnique()
           .HasDatabaseName(
               "UX_Pagos_Unidad_Anio_Mes");

            builder.HasOne(p => p.Unidad)
             .WithMany(u => u.Pagos)
             .HasForeignKey(p => p.IdUnidad)
             .IsRequired()
             .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
