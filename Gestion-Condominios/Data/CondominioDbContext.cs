using Gestion_Condominios.Models;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Condominios.Data
{
    public class CondominioDbContext: DbContext
    {
        public CondominioDbContext(DbContextOptions<CondominioDbContext> options) : base(options) { }

        public DbSet<Condominio> Condominios => Set<Condominio>();
        public DbSet<Pago> Pagos => Set<Pago>();
        public DbSet<Persona> Personas => Set<Persona>();
        public DbSet<Unidad> Unidades => Set<Unidad>();
        public DbSet<UnidadPersona> UnidadesPersonas => Set<UnidadPersona>();
        public DbSet<Visita> Visitas => Set<Visita>();
        public DbSet<Visitante> Visitantes => Set<Visitante>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CondominioDbContext).Assembly);
        }
    }
}
