
using Gestion_Condominios.Data;
using Gestion_Condominios.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Condominios.Controllers
{
    public class VisitasController : Controller
    {
        private readonly CondominioDbContext _context;

        public VisitasController(CondominioDbContext context)
        {
            _context = context;
        }

        // GET: Visitas
        public async Task<IActionResult> Index()
        {
            var visitas = await _context.Visitas
                .Include(v => v.Visitante)
                .Include(v => v.Unidad)
                .ThenInclude(u => u.Condominio)
                .AsNoTracking()
                .OrderByDescending(v => v.FechaHoraIngreso)
                .ToListAsync();

            return View(visitas);
        }

        // GET: Visitas/Create
        public async Task<IActionResult> Create()
        {
            await CargarListas();
            return View();
        }

        // POST: Visitas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int IdVisitante, int IdUnidad)
        {
            bool visitanteExiste = await _context.Visitantes
                .AnyAsync(v => v.IdVisitante == IdVisitante);

            bool unidadExiste = await _context.Unidades
                .AnyAsync(u => u.IdUnidad == IdUnidad);

            if (!visitanteExiste)
                ModelState.AddModelError("IdVisitante",
                    "Seleccioná un visitante válido.");

            if (!unidadExiste)
                ModelState.AddModelError("IdUnidad",
                    "Seleccioná una unidad válida.");

            if (!ModelState.IsValid)
            {
                await CargarListas();
                return View();
            }

            var visita = new Visita
            {
                IdVisitante = IdVisitante,
                IdUnidad = IdUnidad,
                FechaHoraIngreso = DateTime.Now,
                FechaHoraSalida = null
            };

            _context.Visitas.Add(visita);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Ingreso de visita registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListas()
        {
            var visitantes = await _context.Visitantes
                .AsNoTracking()
                .OrderBy(v => v.Apellido)
                .ThenBy(v => v.Nombre)
                .Select(v => new
                {
                    v.IdVisitante,
                    NombreCompleto = v.Nombre + " " +
                                     v.Apellido + " - " + v.NroCI
                })
                .ToListAsync();

            var unidades = await _context.Unidades
                .Include(u => u.Condominio)
                .AsNoTracking()
                .OrderBy(u => u.Condominio.Nombre)
                .ThenBy(u => u.NumeroUnidad)
                .ToListAsync();

            ViewBag.Visitantes = new SelectList(
                visitantes, "IdVisitante", "NombreCompleto");

            ViewBag.Unidades = new SelectList(
                unidades.Select(u => new
                {
                    u.IdUnidad,
                    Descripcion = u.Condominio.Nombre +
                                  " - Casa " + u.NumeroUnidad
                }),
                "IdUnidad", "Descripcion");
        }
    }
}

