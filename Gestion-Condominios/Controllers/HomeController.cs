using Gestion_Condominios.Data;
using Gestion_Condominios.Models;
using Gestion_Condominios.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Gestion_Condominios.Controllers
{
    public class HomeController : Controller
    {
        private readonly CondominioDbContext _context;

        public HomeController(CondominioDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;
            var pendientes = _context.Pagos
                .AsNoTracking()
                .Where(p => p.Estado == "PENDIENTE");

            var modelo = new InicioViewModel
            {
                TotalCondominios = await _context.Condominios.CountAsync(),
                TotalUnidades = await _context.Unidades.CountAsync(),
                // Una casa está ocupada si tiene un INQUILINO; el estado no se guarda en la BD.
                UnidadesOcupadas = await _context.UnidadesPersonas
                    .Where(up => up.TipoRelacion == "INQUILINO")
                    .Select(up => up.IdUnidad)
                    .Distinct()
                    .CountAsync(),
                ExpensasPendientes = await pendientes.CountAsync(),
                // Vencida es una condición calculada: sigue PENDIENTE y su fecha ya pasó.
                ExpensasVencidas = await pendientes.CountAsync(p => p.FechaVencimiento < hoy),
                MontoPendiente = await pendientes.SumAsync(p => (decimal?)p.Monto) ?? 0m,
                VisitasAbiertas = await _context.Visitas.CountAsync(v => v.FechaHoraSalida == null)
            };

            return View(modelo);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
