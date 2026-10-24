using Gestion_Condominios.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gestion_Condominios.Data.Configurations
{
    public class VisitaConfiguration: IEntityTypeConfiguration<Visita>
    {
        public void Configure(EntityTypeBuilder<Visita> builder)
        {
            builder.ToTable("Visitas", t =>
            {
                t.HasCheckConstraint(
               "CK_Visitas_Fechas",
               @"[FechaHoraSalida] IS NULL
                  OR [FechaHoraSalida] >= [FechaHoraIngreso]");
            });

            builder.HasKey(vis => vis.IdVisita);
            builder.Property(v => v.IdVisita)
                    .ValueGeneratedOnAdd();

            builder.Property(vis => vis.FechaHoraIngreso)
                .IsRequired()
                .HasColumnType("datetime2");
            builder.Property(vis => vis.FechaHoraSalida)
                .IsRequired(false)
                .HasColumnType("datetime2");

            builder.HasIndex(v => new
            {
                v.IdUnidad,
                v.FechaHoraIngreso
            })
                .HasDatabaseName(
                    "IX_Visitas_Unidad_FechaIngreso");

            builder.HasIndex(v => new
            {
                v.IdVisitante,
                v.FechaHoraIngreso
            })
                .HasDatabaseName(
                    "IX_Visitas_Visitante_FechaIngreso");

            builder.HasOne(v => v.Unidad)
                .WithMany(u => u.Visitas)
                .HasForeignKey(v => v.IdUnidad)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(v => v.Visitante)
                .WithMany(vi => vi.Visitas)
                .HasForeignKey(v => v.IdVisitante)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
