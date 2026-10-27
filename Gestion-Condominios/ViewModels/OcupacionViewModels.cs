using System.ComponentModel.DataAnnotations;
using Gestion_Condominios.Models;

namespace Gestion_Condominios.ViewModels;

public class OcupacionUnidadViewModel
{
    public Unidad Unidad { get; set; } = null!;
    public IReadOnlyList<UnidadPersona> Ocupantes { get; set; } = [];
    public bool EstaOcupada => Ocupantes.Any();
}

public class AsignarOcupanteViewModel
{
    public int IdUnidad { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una persona.")]
    [Display(Name = "Persona")]
    public int IdPersona { get; set; }

    [Required]
    [RegularExpression("INQUILINO|HABITANTE", ErrorMessage = "El tipo de relación no es válido.")]
    [Display(Name = "Tipo de relación")]
    public string TipoRelacion { get; set; } = "HABITANTE";

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.Today;
}

public class CambiarInquilinoViewModel
{
    public int IdUnidad { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una persona.")]
    [Display(Name = "Nuevo inquilino")]
    public int IdPersona { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.Today;
}
