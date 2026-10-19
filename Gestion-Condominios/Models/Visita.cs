namespace Gestion_Condominios.Models
{
    public class Visita
    {
        public int IdVisitante { get; set; }
        public Visitante Visitante { get; set; } = null!;
        public int IdUnidad { get; set; }
        public Unidad Unidad { get; set; } = null!;
        public DateTime FechaHoraIngreso { get; set; }
        public DateTime? FechaHoraSalida { get; set; }
    }
}
