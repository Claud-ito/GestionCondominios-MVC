using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gestion_Condominios.Data;
using Gestion_Condominios.Models;

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
            return View();
        }

        // POST: Condominios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCondominio,Nombre,Direccion,Telefono")] Condominio condominio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(condominio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(condominio);
        }

        // GET: Condominios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condominio = await _context.Condominios.FindAsync(id);
            if (condominio == null)
            {
                return NotFound();
            }
            return View(condominio);
        }

        // POST: Condominios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCondominio,Nombre,Direccion,Telefono")] Condominio condominio)
        {
            if (id != condominio.IdCondominio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(condominio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CondominioExists(condominio.IdCondominio))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(condominio);
        }

        // GET: Condominios/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Condominios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var condominio = await _context.Condominios.FindAsync(id);
            if (condominio != null)
            {
                _context.Condominios.Remove(condominio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CondominioExists(int id)
        {
            return _context.Condominios.Any(e => e.IdCondominio == id);
        }
    }
}
