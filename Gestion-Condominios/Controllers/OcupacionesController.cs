using Gestion_Condominios.Data;
using Gestion_Condominios.Services;
using Gestion_Condominios.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Condominios.Controllers;

public class OcupacionesController : Controller
{
    private readonly CondominioDbContext _context; private readonly IOcupacionService _service;
    public OcupacionesController(CondominioDbContext context, IOcupacionService service) { _context = context; _service = service; }
    public async Task<IActionResult> Index() => View(await _context.Unidades.AsNoTracking().Include(x => x.Condominio).Select(x => new OcupacionUnidadViewModel { Unidad = x, Ocupantes = x.UnidadPersonas.ToList() }).ToListAsync());
    public async Task<IActionResult> Gestionar(int? id) { if (id is null) return NotFound(); var unidad = await _context.Unidades.AsNoTracking().Include(x => x.Condominio).FirstOrDefaultAsync(x => x.IdUnidad == id); if (unidad is null) return NotFound(); await CargarPersonasAsync(); return View(new OcupacionUnidadViewModel { Unidad = unidad, Ocupantes = await _service.ObtenerOcupantesAsync(unidad.IdUnidad) }); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Asignar(AsignarOcupanteViewModel m) { if (!ModelState.IsValid) return await VolverGestionar(m.IdUnidad, "Revise los datos de la asignación."); var r = m.TipoRelacion == "INQUILINO" ? await _service.AsignarInquilinoAsync(m.IdUnidad,m.IdPersona,m.FechaInicio) : await _service.AgregarHabitanteAsync(m.IdUnidad,m.IdPersona,m.FechaInicio); TempData[r.Exitoso ? "Mensaje Exito" : "Mensaje Error"] = r.Mensaje; return RedirectToAction(nameof(Gestionar),new{id=m.IdUnidad}); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> CambiarInquilino(CambiarInquilinoViewModel m) { if (!ModelState.IsValid) return await VolverGestionar(m.IdUnidad,"Revise los datos del nuevo inquilino.");var r=await _service.CambiarInquilinoAsync(m.IdUnidad,m.IdPersona,m.FechaInicio);TempData[r.Exitoso?"Mensaje Exito":"Mensaje Error"]=r.Mensaje;return RedirectToAction(nameof(Gestionar),new{id=m.IdUnidad}); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> RetirarHabitante(int idUnidad,int idPersona) { var r=await _service.RetirarHabitanteAsync(idUnidad,idPersona);TempData[r.Exitoso?"Mensaje Exito":"Mensaje Error"]=r.Mensaje;return RedirectToAction(nameof(Gestionar),new{id=idUnidad}); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Desocupar(int idUnidad) { var r=await _service.DesocuparAsync(idUnidad);TempData[r.Exitoso?"Mensaje Exito":"Mensaje Error"]=r.Mensaje;return RedirectToAction(nameof(Gestionar),new{id=idUnidad}); }
    private async Task<IActionResult> VolverGestionar(int id,string error){TempData["Mensaje Error"]=error;return RedirectToAction(nameof(Gestionar),new{id});}
    private async Task CargarPersonasAsync() => ViewBag.Personas = new SelectList(await _context.Personas.AsNoTracking().OrderBy(x=>x.Apellido).ThenBy(x=>x.Nombre).Select(x=>new {x.IdPersona, Nombre=$"{x.Apellido}, {x.Nombre} ({x.NroCI})"}).ToListAsync(),"IdPersona","Nombre");
}
