using Microsoft.Identity.Client;

namespace Gestion_Condominios.Models
{
    public class Unidad
    {
        public int IdUnidad { get; set; }
        public string NumeroUnidad { get; set; } = string.Empty;
        public int Piso {  get; set; } 
        public string Tipo { get; set; } = string.Empty;
        public decimal SuperficieM2 { get; set; }
        public int IdCondominio { get; set; }
        public Condominio Condominio { get; set; } = null!;
        public ICollection<UnidadPersona> UnidadPersonas { get; set; } = new List<UnidadPersona>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public ICollection<Visita> Visitas { get; set; } = new List<Visita>();
    }
}
