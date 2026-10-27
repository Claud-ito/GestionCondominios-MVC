using Gestion_Condominios.Models;
using Gestion_Condominios.Services;
using Gestion_Condominios.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Gestion_Condominios.Controllers;

public class PagosController : Controller
{
    private readonly IPagosService _pagos;
    private readonly IGeneradorExpensasService _generador;

    public PagosController(IPagosService pagos, IGeneradorExpensasService generador)
    {
        _pagos = pagos;
        _generador = generador;
    }

    private void Notificar(ResultadoOperacion r) =>
        TempData[r.Exito ? "Mensaje Exito" : "Mensaje Error"] = r.Mensaje;

    // GET: /Pagos
    public async Task<IActionResult> Index([FromQuery] PagoFiltro filtro)
    {
        var vm = new PagoIndexVM
        {
            Filtro = filtro,
            Items = await _pagos.BuscarAsync(filtro),
            Condominios = await _pagos.ListarCondominiosAsync(),
            Unidades = await _pagos.ListarUnidadesAsync(filtro.IdCondominio)
        };
        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var pago = await _pagos.ObtenerAsync(id);
        return pago is null ? NotFound() : View(pago);
    }

    // ---------- Crear manual ----------
    public async Task<IActionResult> Create()
    {
        return View(new PagoCreateVM
        {
            Unidades = await _pagos.ListarUnidadesAsync(soloOcupadas: true),
            Concepto = $"Expensa {DateTime.Today:MM/yyyy}",
            FechaVencimiento = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1)
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PagoCreateVM vm)
    {
        if (ModelState.IsValid)
        {
            var r = await _pagos.CrearManualAsync(vm);
            if (r.Exito) { Notificar(r); return RedirectToAction(nameof(Index)); }
            ModelState.AddModelError(string.Empty, r.Mensaje);
        }
        vm.Unidades = await _pagos.ListarUnidadesAsync(soloOcupadas: true);
        return View(vm);
    }

    // ---------- Editar (solo pendientes) ----------
    public async Task<IActionResult> Edit(int id)
    {
        var p = await _pagos.ObtenerAsync(id);
        if (p is null) return NotFound();
        if (p.Estado == EstadoPago.Pagado)
        {
            TempData["Mensaje Error"] = "Una expensa PAGADA no puede modificarse.";
            return RedirectToAction(nameof(Index));
        }
        return View(new PagoEditVM
        {
            IdPago = p.IdPago, Casa = p.Casa, Periodo = p.Periodo, Monto = p.Monto,
            Concepto = p.Concepto, FechaVencimiento = p.FechaVencimiento
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PagoEditVM vm)
    {
        if (ModelState.IsValid)
        {
            var r = await _pagos.ActualizarAsync(vm);
            Notificar(r);
            if (r.Exito) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, r.Mensaje);
        }
        var p = await _pagos.ObtenerAsync(vm.IdPago);
        if (p is null) return NotFound();
        vm.Casa = p.Casa; vm.Periodo = p.Periodo; vm.Monto = p.Monto;
        return View(vm);
    }

    // ---------- Registrar pago completo ----------
    public async Task<IActionResult> Pagar(int id)
    {
        var p = await _pagos.ObtenerAsync(id);
        if (p is null) return NotFound();
        if (p.Estado == EstadoPago.Pagado)
        {
            TempData["Mensaje Error"] = "La expensa ya fue cancelada.";
            return RedirectToAction(nameof(Index));
        }
        return View(new PagoPagarVM
        {
            IdPago = p.IdPago, Casa = p.Casa, Periodo = p.Periodo,
            Concepto = p.Concepto, Monto = p.Monto, Condicion = p.Condicion
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Pagar(PagoPagarVM vm)
    {
        if (ModelState.IsValid)
        {
            var r = await _pagos.RegistrarPagoAsync(vm.IdPago, vm.FechaPago!.Value);
            Notificar(r);
            if (r.Exito) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, r.Mensaje);
        }
        var p = await _pagos.ObtenerAsync(vm.IdPago);
        if (p is null) return NotFound();
        vm.Casa = p.Casa; vm.Periodo = p.Periodo; vm.Concepto = p.Concepto;
        vm.Monto = p.Monto; vm.Condicion = p.Condicion;
        return View(vm);
    }

    // ---------- Eliminar (solo pendientes) ----------
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _pagos.ObtenerAsync(id);
        return p is null ? NotFound() : View(p);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        Notificar(await _pagos.EliminarAsync(id));
        return RedirectToAction(nameof(Index));
    }

    // ---------- Generador mensual manual ----------
    public IActionResult Generar() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Generar(bool confirmar = true)
    {
        var r = await _generador.GenerarMesActualAsync();
        TempData["Mensaje Exito"] = r.Resumen;
        return RedirectToAction(nameof(Index), new PagoFiltro { Anio = r.Anio, Mes = r.Mes });
    }

    // ---------- Saldos por casa ----------
    public async Task<IActionResult> Saldos(int? idCondominio)
    {
        ViewBag.Condominios = await _pagos.ListarCondominiosAsync();
        ViewBag.IdCondominio = idCondominio;
        return View(await _pagos.ObtenerSaldosAsync(idCondominio));
    }
}
