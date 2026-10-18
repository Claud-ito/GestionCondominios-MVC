namespace Gestion_Condominios.Models
{
    public class Visitante
    {
        public int IdVisitante { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NroCI { get; set; }

        public ICollection<Visita> Visitas {  get; set; }
    }
}

