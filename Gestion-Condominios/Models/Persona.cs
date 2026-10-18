namespace Gestion_Condominios.Models
{
    public class Persona
    {
        public int IdPersona { get; set; }
        public string NroCI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }

        public ICollection<UnidadPersona> UnidadPersonas { get; set; }
    }
}
