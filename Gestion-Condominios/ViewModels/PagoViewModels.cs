using System.ComponentModel.DataAnnotations;
using Gestion_Condominios.Models;
using Gestion_Condominios.ViewModels.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gestion_Condominios.ViewModels;

public static class EstadoPago { public const string Pendiente = "PENDIENTE"; public const string Pagado = "PAGADO"; }

public sealed class PagoFiltro { public int? IdCondominio { get; set; } public int? IdUnidad { get; set; } [Range(1, 9999)] public int? Anio { get; set; } [Range(1, 12)] public int? Mes { get; set; } public string? Estado { get; set; } }

public class PagoItemVM
{
    public int IdPago { get; init; } public string Casa { get; init; } = string.Empty; public string Periodo { get; init; } = string.Empty; public decimal Monto { get; init; } public string Concepto { get; init; } = string.Empty; public DateTime FechaVencimiento { get; init; } public DateTime? FechaPago { get; init; } public string Estado { get; init; } = string.Empty;
    public string Condicion => Estado == EstadoPago.Pagado ? EstadoPago.Pagado : FechaVencimiento.Date < DateTime.Today ? "VENCIDO" : EstadoPago.Pendiente;
    public static PagoItemVM Desde(Pago p) => new() { IdPago = p.IdPago, Casa = p.Unidad.Condominio.Nombre + " - Unidad " + p.Unidad.NumeroUnidad, Periodo = $"{p.Mes:D2}/{p.Anio}", Monto = p.Monto, Concepto = p.Concepto, FechaVencimiento = p.FechaVencimiento, FechaPago = p.FechaPago, Estado = p.Estado };
}
public sealed class PagoDetalleVM : PagoItemVM { public static new PagoDetalleVM Desde(Pago p) => new() { IdPago = p.IdPago, Casa = p.Unidad.Condominio.Nombre + " - Unidad " + p.Unidad.NumeroUnidad, Periodo = $"{p.Mes:D2}/{p.Anio}", Monto = p.Monto, Concepto = p.Concepto, FechaVencimiento = p.FechaVencimiento, FechaPago = p.FechaPago, Estado = p.Estado }; }
public class PagoIndexVM { public PagoFiltro Filtro { get; set; } = new(); public IReadOnlyList<PagoItemVM> Items { get; set; } = []; public IReadOnlyList<SelectListItem> Condominios { get; set; } = []; public IReadOnlyList<SelectListItem> Unidades { get; set; } = []; }
public sealed class ReporteCobranzaVM : PagoIndexVM { public decimal TotalFacturado => Items.Sum(x => x.Monto); public decimal TotalCobrado => Items.Where(x => x.Estado == EstadoPago.Pagado).Sum(x => x.Monto); public decimal TotalPendienteOVencido => TotalFacturado - TotalCobrado; public int CantidadExpensas => Items.Count; }
public sealed class PagoCreateVM { [Range(1, int.MaxValue)] public int IdUnidad { get; set; } [Range(1, 9999)] public int Anio { get; set; } = DateTime.Today.Year; [Range(1, 12)] public int Mes { get; set; } = DateTime.Today.Month; [Range(typeof(decimal), "0.01", "9999999999999999", ParseLimitsInInvariantCulture = true)] [ModelBinder(BinderType = typeof(DecimalFlexibleModelBinder))] public decimal Monto { get; set; } [Required, StringLength(200)] public string Concepto { get; set; } = string.Empty; [DataType(DataType.Date), Required] public DateTime FechaVencimiento { get; set; } public IReadOnlyList<SelectListItem> Unidades { get; set; } = []; }
public sealed class PagoEditVM { public int IdPago { get; set; } public string Casa { get; set; } = string.Empty; public string Periodo { get; set; } = string.Empty; public decimal Monto { get; set; } [Required, StringLength(200)] public string Concepto { get; set; } = string.Empty; [DataType(DataType.Date), Required] public DateTime FechaVencimiento { get; set; } }
public sealed class PagoPagarVM { public int IdPago { get; set; } public string Casa { get; set; } = string.Empty; public string Periodo { get; set; } = string.Empty; public string Concepto { get; set; } = string.Empty; public decimal Monto { get; set; } public string Condicion { get; set; } = string.Empty; [DataType(DataType.Date), Required] public DateTime? FechaPago { get; set; } = DateTime.Today; }
public sealed class SaldoUnidadVM { public string Condominio { get; set; } = string.Empty; public string Unidad { get; set; } = string.Empty; public decimal TotalFacturado { get; set; } public decimal TotalCobrado { get; set; } public decimal TotalPendiente { get; set; } }
