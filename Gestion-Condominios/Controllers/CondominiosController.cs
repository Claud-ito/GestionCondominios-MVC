using Gestion_Condominios.Data;
using Gestion_Condominios.Models;
using Gestion_Condominios.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gestion_Condominios.Controllers
{
    public class CondominiosController : Controller
    {
        private readonly CondominioDbContext _context;

        public CondominiosController(CondominioDbContext context)
        {
            _context = context;
        }

        // GET: Condominios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Condominios.ToListAsync());
        }

        // GET: Condominios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condominio = await _context.Condominios
                .FirstOrDefaultAsync(m => m.IdCondominio == id);
            if (condominio == null)
            {
                return NotFound();
            }

            return View(condominio);
        }

        // GET: Condominios/Create
        public IActionResult Create()
        {
            var modelo = new CondominioCreateViewModel();
            return View(modelo);
        }

        // POST: Condominios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CondominioCreateViewModel modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo.Nombre))
            {
                ModelState.AddModelError(
                    nameof(modelo.Nombre),
                    "El nombre no puede estar vacío.");
            }

            if (string.IsNullOrWhiteSpace(modelo.Direccion))
            {
                ModelState.AddModelError(
                    nameof(modelo.Direccion),
                    "La dirección no puede estar vacía.");
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var condominio = new Condominio
            {
                Nombre = modelo.Nombre.Trim(),
                Direccion = modelo.Direccion.Trim(),
                Telefono = string.IsNullOrWhiteSpace(modelo.Telefono) ? null : modelo.Telefono.Trim(),  
            };

            _context.Condominios.Add(condominio);
            await _context.SaveChangesAsync();

            TempData["Mensaje Exito"] = $"El condominio {modelo.Nombre} se registro correctamente";

            return RedirectToAction(nameof(Index));
        }

        // GET: Condominios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condominio = await _context.Condominios
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCondominio == id.Value);
            if (condominio == null)
            {
                return NotFound();
            }
            var modelo = new CondominioEditViewModel
            {
                IdCondominio = condominio.IdCondominio,
                Nombre = condominio.Nombre,
                Direccion = condominio.Direccion,
                Telefono = condominio.Telefono
            };

            return View(modelo);
        }

        // POST: Condominios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CondominioEditViewModel modelo)
        {
            if (id != modelo.IdCondominio)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(modelo.Nombre))
            {
                ModelState.AddModelError(
                    nameof(modelo.Nombre),
                    "El nombre no puede estar vacío.");
            }

            if (string.IsNullOrWhiteSpace(modelo.Direccion))
            {
                ModelState.AddModelError(
                    nameof(modelo.Direccion),
                    "La dirección no puede estar vacía.");
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var condominio = await _context.Condominios.FindAsync(id);

            if (condominio == null)
            {
                return NotFound();
            }

            condominio.Nombre = modelo.Nombre.Trim();
            condominio.Direccion = modelo.Direccion.Trim();
            condominio.Telefono =
                string.IsNullOrWhiteSpace(modelo.Telefono)
                    ? null
                    : modelo.Telefono.Trim();


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Condominios
                    .AnyAsync(c => c.IdCondominio == id))
                {
                    return NotFound();
                }

                throw;
            }

            TempData["Mensaje Exito"] =
                $"El condominio '{condominio.Nombre}' se actualizó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Condominios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelo = await _context.Condominios
                .AsNoTracking()
                .Where(c => c.IdCondominio == id.Value)
                .Select(c => new CondominioDeleteViewModel {
                    IdCondominio = c.IdCondominio,
                    Nombre = c.Nombre,
                    CantidadUnidades = c.Unidades.Count()
                })
                .FirstOrDefaultAsync();

            if (modelo == null)
            {
                return NotFound();
            }

            return View(modelo);
        }

        // POST: Condominios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var condominio = await _context.Condominios.FindAsync(id);
            if (condominio == null)
            {
                return NotFound();
            }

            bool tieneUnidades = await _context.Unidades
                .AnyAsync(u => u.IdCondominio == id);

            if (tieneUnidades)
            {
                TempData["Mensaje Error"] =
                    "No se puede eliminar el condominio porque tiene unidades asociadas.";

                return RedirectToAction(nameof(Delete), new { id });
            }


            try
            {
                _context.Condominios.Remove(condominio);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            catch (DbUpdateException ex) when (
                    ex.GetBaseException() is SqlException sqlEx &&
                    sqlEx.Number == 547)
            {
                TempData["Mensaje Error"] =
                    "No se pudo eliminar el condominio porque existen registros relacionados.";

                return RedirectToAction(nameof(Delete), new { id });
            }

            TempData["Mensaje Exito"] =
                            $"El condominio '{condominio.Nombre}' se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        private bool CondominioExists(int id)
        {
            return _context.Condominios.Any(e => e.IdCondominio == id);
        }
    }
}
