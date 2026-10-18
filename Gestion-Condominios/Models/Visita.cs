namespace Gestion_Condominios.Models
{
    public class Visita
    {
        public int IdVisitante { get; set; }
        public Visitante Visitante { get; set; }
        public int IdUnidad { get; set; }
        public  Unidad Unidad   { get; set; }
        public DateTime FechaHoraIngreso { get; set; }
        public DateTime? FechaHoraSalida { get; set; }
    }
}
