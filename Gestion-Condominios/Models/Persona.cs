namespace Gestion_Condominios.Models
{
    public class Persona
    {
        public int IdPersona { get; set; }
        public string NroCI { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Telefono { get; set; } 
        public string? Correo { get; set; } 

        public ICollection<UnidadPersona> UnidadPersonas { get; set; } = new List<UnidadPersona>();
    }
}
