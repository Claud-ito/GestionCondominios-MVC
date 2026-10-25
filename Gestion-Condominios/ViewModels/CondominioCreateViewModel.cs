using System.ComponentModel.DataAnnotations;

namespace Gestion_Condominios.ViewModels
{
    public class CondominioCreateViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(80, ErrorMessage = "El nombre no puede contener mas de 80 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(150, ErrorMessage = "La direccion no puede contener mas de 150 caracteres.")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(8, ErrorMessage = "El Nro. telefonico no puede contener mas de 8 caracteres.")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }
    }
}
