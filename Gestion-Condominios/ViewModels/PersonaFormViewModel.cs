using System.ComponentModel.DataAnnotations;

namespace Gestion_Condominios.ViewModels;

public class PersonaFormViewModel
{
    public int IdPersona { get; set; }

    [Required(ErrorMessage = "El documento es obligatorio.")]
    [StringLength(10)]
    [Display(Name = "Nro. CI")]
    public string NroCI { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(25)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(40)]
    public string Apellido { get; set; } = string.Empty;

    [StringLength(8)]
    public string? Telefono { get; set; }

    [StringLength(80)]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    public string? Correo { get; set; }
}
