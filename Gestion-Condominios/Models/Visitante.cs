namespace Gestion_Condominios.Models
{
    public class Visitante
    {
        public int IdVisitante { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string NroCI { get; set; } = string.Empty;

        public ICollection<Visita> Visitas {  get; set; } = new List<Visita>();
    }
}

