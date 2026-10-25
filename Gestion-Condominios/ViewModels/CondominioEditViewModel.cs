using System.ComponentModel.DataAnnotations;

namespace Gestion_Condominios.ViewModels
{
    public class CondominioEditViewModel
    {
        public int IdCondominio { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(80, ErrorMessage = "El nombre no puede superar los 80 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(150, ErrorMessage = "La dirección no puede superar los 150 caracteres.")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(8, ErrorMessage =
            "El teléfono no puede superar los 8 caracteres.")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }
    }
}
