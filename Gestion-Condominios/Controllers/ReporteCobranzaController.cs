using Gestion_Condominios.Services;
using Gestion_Condominios.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Gestion_Condominios.Controllers;

/// <summary>Reporte 1: Expensas y cobranza por período.</summary>
public class ReporteCobranzaController : Controller
{
    private readonly IPagosService _pagos;
    public ReporteCobranzaController(IPagosService pagos) => _pagos = pagos;

    public async Task<IActionResult> Index([FromQuery] PagoFiltro filtro)
    {
        // Primera visita: sin filtros muestra el mes actual.
        if (!Request.Query.Any())
        {
            filtro.Anio = DateTime.Today.Year;
            filtro.Mes = DateTime.Today.Month;
        }

        var vm = new ReporteCobranzaVM
        {
            Filtro = filtro,
            Items = await _pagos.BuscarAsync(filtro),
            Condominios = await _pagos.ListarCondominiosAsync(),
            Unidades = await _pagos.ListarUnidadesAsync(filtro.IdCondominio)
        };
        return View(vm);
    }
}
