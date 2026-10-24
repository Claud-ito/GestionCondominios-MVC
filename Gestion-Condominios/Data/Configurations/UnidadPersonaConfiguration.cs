using Gestion_Condominios.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gestion_Condominios.Data.Configurations
{
    public class UnidadPersonaConfiguration: IEntityTypeConfiguration<UnidadPersona>
    {
        public void Configure(EntityTypeBuilder<UnidadPersona> builder)
        {
            builder.ToTable("UnidadPersonas", t =>
            {
                t.HasCheckConstraint(
                    "CK_UnidadPersonas_TipoRelacion",
                    "[TipoRelacion] IN ('INQUILINO', 'HABITANTE')");
            });
            builder.HasKey(up => new { up.IdUnidad, up.IdPersona});

            builder.Property(up => up.TipoRelacion)
                .IsRequired()
                .HasMaxLength(15);
            builder.Property(up => up.FechaInicio)
                .IsRequired()
                .HasColumnType("date");

            builder.HasIndex(up => up.IdUnidad)
                .IsUnique()
                .HasFilter("[TipoRelacion] = 'INQUILINO'")
                .HasDatabaseName(
               "UX_UnidadPersonas_Inquilino_Unidad");

            builder.HasIndex(up => up.IdPersona)
                .HasDatabaseName(
                "IX_UnidadPersonas_IdPersona");


            builder.HasOne(up => up.Unidad)
            .WithMany(u => u.UnidadPersonas)
            .HasForeignKey(up => up.IdUnidad)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(up => up.Persona)
           .WithMany(p => p.UnidadPersonas)
           .HasForeignKey(up => up.IdPersona)
           .IsRequired()
           .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
