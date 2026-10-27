
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
        // Historial, búsqueda y filtros
        public async Task<IActionResult> Index(
            string? buscar, string? estado, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var consulta = _context.Visitas
                .Include(v => v.Visitante)
                .Include(v => v.Unidad)
                .ThenInclude(u => u.Condominio)
                .AsNoTracking()
                .AsQueryable();

            // Buscar por nombre, apellido o CI
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();

                consulta = consulta.Where(v =>
                    v.Visitante.Nombre.Contains(buscar) ||
                    v.Visitante.Apellido.Contains(buscar) ||
                    v.Visitante.NroCI.Contains(buscar));
            }

            // Filtrar por estado de la visita
            if (estado == "EnCurso")
            {
                consulta = consulta.Where(v =>
                    v.FechaHoraSalida == null);
            }
            else if (estado == "Finalizada")
            {
                consulta = consulta.Where(v =>
                    v.FechaHoraSalida != null);
            }

            if (fechaDesde.HasValue)
                consulta = consulta.Where(v => v.FechaHoraIngreso >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
            {
                var hastaExclusiva = fechaHasta.Value.Date.AddDays(1);
                consulta = consulta.Where(v => v.FechaHoraIngreso < hastaExclusiva);
            }

            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value.Date > fechaHasta.Value.Date)
                ModelState.AddModelError(string.Empty, "La fecha desde no puede ser posterior a la fecha hasta.");

            ViewBag.Buscar = buscar;
            ViewBag.Estado = estado;
            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");

            var visitas = await consulta
                .OrderByDescending(v => v.FechaHoraIngreso)
                .ToListAsync();

            ViewBag.TotalVisitas = visitas.Count;
            ViewBag.VisitasAbiertas = visitas.Count(v => v.FechaHoraSalida is null);

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
            {
                ModelState.AddModelError(
                    "IdVisitante",
                    "Seleccioná un visitante válido.");
            }

            if (!unidadExiste)
            {
                ModelState.AddModelError(
                    "IdUnidad",
                    "Seleccioná una unidad válida.");
            }

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

        // POST: Visitas/RegistrarSalida
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarSalida(int id)
        {
            var visita = await _context.Visitas
                .FirstOrDefaultAsync(v => v.IdVisita == id);

            if (visita == null)
            {
                return NotFound();
            }

            if (visita.FechaHoraSalida != null)
            {
                TempData["Error"] =
                    "Esta visita ya tiene una salida registrada.";

                return RedirectToAction(nameof(Index));
            }

            visita.FechaHoraSalida = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Salida de visita registrada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Cargar visitantes y unidades
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
                visitantes,
                "IdVisitante",
                "NombreCompleto");

            ViewBag.Unidades = new SelectList(
                unidades.Select(u => new
                {
                    u.IdUnidad,
                    Descripcion = u.Condominio.Nombre +
                                  " - Casa " + u.NumeroUnidad
                }),
                "IdUnidad",
                "Descripcion");
        }
    }
}
