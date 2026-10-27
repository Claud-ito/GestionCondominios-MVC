using System.ComponentModel.DataAnnotations;
using Gestion_Condominios.ViewModels.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace Gestion_Condominios.ViewModels;

public class UnidadFormViewModel
{
    public int IdUnidad { get; set; }

    [Display(Name = "Condominio")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un condominio.")]
    public int IdCondominio { get; set; }

    [Required(ErrorMessage = "El número de unidad es obligatorio.")]
    [StringLength(20)]
    [Display(Name = "Número")]
    public string NumeroUnidad { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "La cantidad de cuartos no puede ser negativa.")]
    [Display(Name = "Cantidad de cuartos")]
    public int CantidadCuartos { get; set; }

    [Required(ErrorMessage = "El tipo es obligatorio.")]
    [StringLength(50)]
    public string Tipo { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "99999999", ParseLimitsInInvariantCulture = true, ErrorMessage = "La superficie debe ser mayor a cero.")]
    [ModelBinder(BinderType = typeof(DecimalFlexibleModelBinder))]
    [Display(Name = "Superficie (m²)")]
    public decimal SuperficieM2 { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999999999", ParseLimitsInInvariantCulture = true, ErrorMessage = "La tarifa mensual debe ser mayor a cero.")]
    [ModelBinder(BinderType = typeof(DecimalFlexibleModelBinder))]
    [Display(Name = "Tarifa mensual")]
    public decimal PagoMes { get; set; }
}
