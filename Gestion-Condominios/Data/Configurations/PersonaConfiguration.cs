using Gestion_Condominios.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gestion_Condominios.Data.Configurations
{
    public class PersonaConfiguration: IEntityTypeConfiguration<Persona>
    {
        public void Configure(EntityTypeBuilder<Persona> builder)
        {
            builder.ToTable("Personas", t =>
            {
                t.HasCheckConstraint(
                "CK_Personas_NroCI",
                "LEN(LTRIM(RTRIM([NroCI]))) > 0");

                t.HasCheckConstraint(
                    "CK_Personas_Nombre",
                    "LEN(LTRIM(RTRIM([Nombre]))) > 0");

                t.HasCheckConstraint(
                    "CK_Personas_Apellido",
                    "LEN(LTRIM(RTRIM([Apellido]))) > 0");
            });

            builder.HasKey(p => p.IdPersona);
            builder.Property(p => p.IdPersona)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.NroCI)
               .IsRequired()
               .HasMaxLength(10);
            builder.Property(p => p.Nombre)
                .IsRequired()
                   .HasMaxLength(25);
            builder.Property(p => p.Apellido)
                .IsRequired()
                .HasMaxLength(40);
            builder.Property(p => p.Correo)
                .IsRequired(false)
                .HasMaxLength(80);
            builder.Property(p => p.Telefono)
                .IsRequired(false)
                .HasMaxLength(8);

            builder.HasIndex(p => p.NroCI)
                .IsUnique()
                .HasDatabaseName("UX_Personas_NroCI");

        }
    }
}
