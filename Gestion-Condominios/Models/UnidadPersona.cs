namespace Gestion_Condominios.Models
{
    public class UnidadPersona
    {
        public int IdUnidad {  get; set; }
        public Unidad Unidad { get; set; } = null!;
        public int IdPersona { get; set; }
        public Persona Persona { get; set; } = null!;
        public string TipoRelacion { get; set; } = string.Empty;         //INQUILINO, HABITANTE
        public DateTime FechaInicio { get; set; }
    }
}
