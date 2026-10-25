namespace Gestion_Condominios.Models
{
    public class Condominio
    {
        public int IdCondominio { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string? Telefono { get; set; } 
        //ICollection nos ayudara en el desacoplamiento de dependencias de los objetos
        public ICollection<Unidad> Unidades { get; set; } = new List<Unidad>();
    }
}
