namespace Gestion_Condominios.Models
{
    public class UnidadPersona
    {
        public int IdUnidad {  get; set; }
        public Unidad Unidad { get; set; }
        public int IdPersona { get; set; }
        public Persona Persona { get; set; }
        public string TipoRelacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin {  get; set; }
    }
}
