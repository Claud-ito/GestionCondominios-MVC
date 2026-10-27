using Gestion_Condominios.Data;
using Gestion_Condominios.Models;
using Gestion_Condominios.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Condominios.Controllers;

public class PersonasController : Controller
{
    private readonly CondominioDbContext _context;
    public PersonasController(CondominioDbContext context) => _context = context;
    public async Task<IActionResult> Index(string? buscar) { var q = _context.Personas.AsNoTracking(); if (!string.IsNullOrWhiteSpace(buscar)) { buscar = buscar.Trim(); q = q.Where(x => x.NroCI.Contains(buscar) || x.Nombre.Contains(buscar) || x.Apellido.Contains(buscar)); } ViewBag.Buscar = buscar; return View(await q.OrderBy(x => x.Apellido).ThenBy(x => x.Nombre).ToListAsync()); }
    public async Task<IActionResult> Details(int? id) { if (id is null) return NotFound(); var p = await _context.Personas.AsNoTracking().Include(x => x.UnidadPersonas).ThenInclude(x => x.Unidad).ThenInclude(x => x.Condominio).FirstOrDefaultAsync(x => x.IdPersona == id); return p is null ? NotFound() : View(p); }
    public IActionResult Create() => View(new PersonaFormViewModel());
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Create(PersonaFormViewModel m) { Normalizar(m); await ValidarDocumentoAsync(m); if (!ModelState.IsValid) return View(m); _context.Personas.Add(Mapear(m)); try { await _context.SaveChangesAsync(); } catch (DbUpdateException ex) when (Duplicado(ex)) { ModelState.AddModelError(nameof(m.NroCI), "Ya existe una persona con este documento."); return View(m); } TempData["Mensaje Exito"] = "La persona se registró correctamente."; return RedirectToAction(nameof(Index)); }
    public async Task<IActionResult> Edit(int? id) { if (id is null) return NotFound(); var p = await _context.Personas.AsNoTracking().FirstOrDefaultAsync(x => x.IdPersona == id); return p is null ? NotFound() : View(new PersonaFormViewModel { IdPersona = p.IdPersona, NroCI = p.NroCI, Nombre = p.Nombre, Apellido = p.Apellido, Telefono = p.Telefono, Correo = p.Correo }); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Edit(int id, PersonaFormViewModel m) { if (id != m.IdPersona) return NotFound(); Normalizar(m); await ValidarDocumentoAsync(m, id); if (!ModelState.IsValid) return View(m); var p = await _context.Personas.FindAsync(id); if (p is null) return NotFound(); p.NroCI=m.NroCI;p.Nombre=m.Nombre;p.Apellido=m.Apellido;p.Telefono=m.Telefono;p.Correo=m.Correo; try { await _context.SaveChangesAsync(); } catch (DbUpdateException ex) when (Duplicado(ex)) { ModelState.AddModelError(nameof(m.NroCI), "Ya existe una persona con este documento."); return View(m); } TempData["Mensaje Exito"]="La persona se actualizó correctamente.";return RedirectToAction(nameof(Index)); }
    public async Task<IActionResult> Delete(int? id) => await Details(id);
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken] public async Task<IActionResult> DeleteConfirmed(int id) { var p=await _context.Personas.FindAsync(id);if(p is null)return NotFound();if(await _context.UnidadesPersonas.AnyAsync(x=>x.IdPersona==id)){TempData["Mensaje Error"]="No se puede eliminar una persona con relaciones de ocupación activas.";return RedirectToAction(nameof(Details),new{id});}_context.Personas.Remove(p);await _context.SaveChangesAsync();TempData["Mensaje Exito"]="La persona se eliminó correctamente.";return RedirectToAction(nameof(Index)); }
    private async Task ValidarDocumentoAsync(PersonaFormViewModel m,int? excluir=null) { if(await _context.Personas.AnyAsync(x=>x.NroCI==m.NroCI&&x.IdPersona!=excluir))ModelState.AddModelError(nameof(m.NroCI),"Ya existe una persona con este documento."); }
    private static Persona Mapear(PersonaFormViewModel m)=>new(){NroCI=m.NroCI,Nombre=m.Nombre,Apellido=m.Apellido,Telefono=m.Telefono,Correo=m.Correo};
    private static void Normalizar(PersonaFormViewModel m){m.NroCI=m.NroCI?.Trim()??"";m.Nombre=m.Nombre?.Trim()??"";m.Apellido=m.Apellido?.Trim()??"";m.Telefono=string.IsNullOrWhiteSpace(m.Telefono)?null:m.Telefono.Trim();m.Correo=string.IsNullOrWhiteSpace(m.Correo)?null:m.Correo.Trim();}
    private static bool Duplicado(DbUpdateException e)=>e.GetBaseException() is SqlException {Number:2601 or 2627};
}
