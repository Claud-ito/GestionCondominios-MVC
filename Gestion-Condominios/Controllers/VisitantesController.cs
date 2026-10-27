
using Gestion_Condominios.Data;
using Gestion_Condominios.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Condominios.Controllers
{
    public class VisitantesController : Controller
    {
        private readonly CondominioDbContext _context;

        public VisitantesController(CondominioDbContext context)
        {
            _context = context;
        }

        // GET: Visitantes
        public async Task<IActionResult> Index()
        {
            var visitantes = await _context.Visitantes
                .AsNoTracking()
                .OrderBy(v => v.Apellido)
                .ThenBy(v => v.Nombre)
                .ToListAsync();

            return View(visitantes);
        }

        // GET: Visitantes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Visitantes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Visitante visitante)
        {
            visitante.Nombre = visitante.Nombre?.Trim() ?? "";
            visitante.Apellido = visitante.Apellido?.Trim() ?? "";
            visitante.NroCI = visitante.NroCI?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(visitante.Nombre))
                ModelState.AddModelError(nameof(Visitante.Nombre),
                    "El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(visitante.Apellido))
                ModelState.AddModelError(nameof(Visitante.Apellido),
                    "El apellido es obligatorio.");

            if (string.IsNullOrWhiteSpace(visitante.NroCI))
                ModelState.AddModelError(nameof(Visitante.NroCI),
                    "El número de CI es obligatorio.");

            if (!ModelState.IsValid)
                return View(visitante);

            bool existe = await _context.Visitantes
                .AnyAsync(v => v.NroCI == visitante.NroCI);

            if (existe)
            {
                ModelState.AddModelError(nameof(Visitante.NroCI),
                    "Ya existe un visitante registrado con este CI.");

                return View(visitante);
            }

            try
            {
                _context.Visitantes.Add(visitante);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("",
                    "No se pudo guardar el visitante. Verificá que el CI no esté duplicado.");
                return View(visitante);
            }

            TempData["Mensaje"] = "Visitante registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}
