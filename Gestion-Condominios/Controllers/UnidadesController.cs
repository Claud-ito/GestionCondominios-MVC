using Gestion_Condominios.Data;
using Gestion_Condominios.Models;
using Gestion_Condominios.Services;
using Gestion_Condominios.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Condominios.Controllers;

public class UnidadesController : Controller
{
    private readonly CondominioDbContext _context;
    private readonly IOcupacionService _ocupacion;
    public UnidadesController(CondominioDbContext context, IOcupacionService ocupacion) { _context = context; _ocupacion = ocupacion; }

    public async Task<IActionResult> Index() => View(await _context.Unidades.AsNoTracking().Include(x => x.Condominio)
        .Select(x => new OcupacionUnidadViewModel { Unidad = x, Ocupantes = x.UnidadPersonas.ToList() }).ToListAsync());

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var unidad = await _context.Unidades.AsNoTracking().Include(x => x.Condominio).FirstOrDefaultAsync(x => x.IdUnidad == id);
        if (unidad is null) return NotFound();
        return View(new OcupacionUnidadViewModel { Unidad = unidad, Ocupantes = await _ocupacion.ObtenerOcupantesAsync(unidad.IdUnidad) });
    }

    public async Task<IActionResult> Create() { await CargarCondominiosAsync(); return View(new UnidadFormViewModel()); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UnidadFormViewModel modelo)
    {
        Normalizar(modelo); await ValidarUnidadAsync(modelo);
        if (!ModelState.IsValid) { await CargarCondominiosAsync(modelo.IdCondominio); return View(modelo); }
        _context.Unidades.Add(Mapear(modelo));
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateException ex) when (EsDuplicado(ex)) { ModelState.AddModelError(nameof(modelo.NumeroUnidad), "Ya existe una unidad con ese número en el condominio seleccionado."); await CargarCondominiosAsync(modelo.IdCondominio); return View(modelo); }
        TempData["Mensaje Exito"] = "La unidad se registró correctamente."; return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound(); var u = await _context.Unidades.AsNoTracking().FirstOrDefaultAsync(x => x.IdUnidad == id);
        if (u is null) return NotFound(); await CargarCondominiosAsync(u.IdCondominio); return View(new UnidadFormViewModel { IdUnidad = u.IdUnidad, IdCondominio = u.IdCondominio, NumeroUnidad = u.NumeroUnidad, CantidadCuartos = u.CantidadCuartos, Tipo = u.Tipo, SuperficieM2 = u.SuperficieM2, PagoMes = u.PagoMes });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UnidadFormViewModel modelo)
    {
        if (id != modelo.IdUnidad) return NotFound(); Normalizar(modelo); await ValidarUnidadAsync(modelo, id);
        if (!ModelState.IsValid) { await CargarCondominiosAsync(modelo.IdCondominio); return View(modelo); }
        var unidad = await _context.Unidades.FindAsync(id); if (unidad is null) return NotFound();
        unidad.IdCondominio = modelo.IdCondominio; unidad.NumeroUnidad = modelo.NumeroUnidad; unidad.CantidadCuartos = modelo.CantidadCuartos; unidad.Tipo = modelo.Tipo; unidad.SuperficieM2 = modelo.SuperficieM2; unidad.PagoMes = modelo.PagoMes;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateException ex) when (EsDuplicado(ex)) { ModelState.AddModelError(nameof(modelo.NumeroUnidad), "Ya existe una unidad con ese número en el condominio seleccionado."); await CargarCondominiosAsync(modelo.IdCondominio); return View(modelo); }
        catch (DbUpdateConcurrencyException) { ModelState.AddModelError("", "La unidad fue modificada por otra operación. Vuelva a intentarlo."); await CargarCondominiosAsync(modelo.IdCondominio); return View(modelo); }
        TempData["Mensaje Exito"] = "La unidad se actualizó correctamente."; return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id) => id is null ? NotFound() : await Details(id);

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var unidad = await _context.Unidades.FindAsync(id); if (unidad is null) return NotFound();
        if (await _context.UnidadesPersonas.AnyAsync(x => x.IdUnidad == id) || await _context.Pagos.AnyAsync(x => x.IdUnidad == id) || await _context.Visitas.AnyAsync(x => x.IdUnidad == id))
        { TempData["Mensaje Error"] = "No se puede eliminar la unidad porque tiene ocupaciones, pagos o visitas relacionados."; return RedirectToAction(nameof(Details), new { id }); }
        _context.Unidades.Remove(unidad); await _context.SaveChangesAsync(); TempData["Mensaje Exito"] = "La unidad se eliminó correctamente."; return RedirectToAction(nameof(Index));
    }

    private async Task ValidarUnidadAsync(UnidadFormViewModel m, int? excluir = null)
    {
        if (!await _context.Condominios.AnyAsync(x => x.IdCondominio == m.IdCondominio)) ModelState.AddModelError(nameof(m.IdCondominio), "El condominio seleccionado no existe.");
        if (await _context.Unidades.AnyAsync(x => x.IdCondominio == m.IdCondominio && x.NumeroUnidad == m.NumeroUnidad && x.IdUnidad != excluir)) ModelState.AddModelError(nameof(m.NumeroUnidad), "Ya existe una unidad con ese número en el condominio seleccionado.");
    }
    private async Task CargarCondominiosAsync(int? seleccionado = null) => ViewBag.Condominios = new SelectList(await _context.Condominios.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync(), "IdCondominio", "Nombre", seleccionado);
    private static Unidad Mapear(UnidadFormViewModel m) => new() { IdCondominio = m.IdCondominio, NumeroUnidad = m.NumeroUnidad, CantidadCuartos = m.CantidadCuartos, Tipo = m.Tipo, SuperficieM2 = m.SuperficieM2, PagoMes = m.PagoMes };
    private static void Normalizar(UnidadFormViewModel m) { m.NumeroUnidad = m.NumeroUnidad?.Trim() ?? ""; m.Tipo = m.Tipo?.Trim() ?? ""; }
    private static bool EsDuplicado(DbUpdateException ex) => ex.GetBaseException() is SqlException { Number: 2601 or 2627 };
}
