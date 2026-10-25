using Gestion_Condominios.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gestion_Condominios.Data.Configurations
{
    public class CondominioConfiguration: IEntityTypeConfiguration<Condominio>
    {
        public void Configure(EntityTypeBuilder<Condominio> builder) {
            builder.ToTable("Condominios", t =>
            {
                t.HasCheckConstraint("CK_Condominios_Nombre",
                "LEN(LTRIM(RTRIM([Nombre]))) > 0");
                t.HasCheckConstraint(
                "CK_Condominios_Direccion",
                "LEN(LTRIM(RTRIM([Direccion]))) > 0");

            }); 
            builder.HasKey(c => c.IdCondominio);

            builder.Property(c => c.IdCondominio)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(80);
            builder.Property(c => c.Direccion)
                .IsRequired()
                .HasMaxLength(150);
            builder.Property(c => c.Telefono)
                .IsRequired(false)
                .HasMaxLength(8);
        }
    }
}
