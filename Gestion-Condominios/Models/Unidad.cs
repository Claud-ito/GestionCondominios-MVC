using Microsoft.Identity.Client;

namespace Gestion_Condominios.Models
{
    public class Unidad
    {
        public int IdUnidad { get; set; }
        public string NumeroUnidad { get; set; }
        public int Piso {  get; set; }
        public string Tipo { get; set; }
        public decimal SuperficieM2 { get; set; }
        public int IdCondominio { get; set; }
        public Condominio Condominio { get; set; }
        public ICollection<UnidadPersona> UnidadPersonas { get; set; }
        public ICollection<Pago> Pagos { get; set; }
        public ICollection<Visita> Visitas { get; set; }
    }
}
