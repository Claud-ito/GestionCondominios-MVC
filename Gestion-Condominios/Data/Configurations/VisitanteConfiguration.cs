using Gestion_Condominios.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gestion_Condominios.Data.Configurations
{
    public class VisitanteConfiguration: IEntityTypeConfiguration<Visitante>
    {
        public void Configure(EntityTypeBuilder<Visitante> builder)
        {
            builder.ToTable("Visitantes", t =>
            {
                t.HasCheckConstraint(
                "CK_Visitantes_Nombre",
                "LEN(LTRIM(RTRIM([Nombre]))) > 0");

                t.HasCheckConstraint(
                    "CK_Visitantes_Apellido",
                    "LEN(LTRIM(RTRIM([Apellido]))) > 0");

                t.HasCheckConstraint(
                    "CK_Visitantes_NroCI",
                    "LEN(LTRIM(RTRIM([NroCI]))) > 0");
            });

            builder.HasKey(v => v.IdVisitante);
            builder.Property(v => v.IdVisitante)
                .ValueGeneratedOnAdd();

            builder.Property(v => v.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Apellido)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.NroCI)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(v => v.NroCI)
                .IsUnique()
                .HasDatabaseName(
                "UX_Visitantes_NroCI");
        }
    }
}
