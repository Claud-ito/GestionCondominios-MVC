using Gestion_Condominios.Data;
using Gestion_Condominios.Models;
using Gestion_Condominios.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Condominios.Services;

public class PagosService : IPagosService, IGeneradorExpensasService
{
    private readonly CondominioDbContext _context;
    public PagosService(CondominioDbContext context) => _context = context;

    public async Task<IReadOnlyList<PagoItemVM>> BuscarAsync(PagoFiltro filtro)
    {
        var pagos = _context.Pagos.AsNoTracking().Include(p => p.Unidad).ThenInclude(u => u.Condominio).AsQueryable();
        if (filtro.IdCondominio.HasValue) pagos = pagos.Where(p => p.Unidad.IdCondominio == filtro.IdCondominio);
        if (filtro.IdUnidad.HasValue) pagos = pagos.Where(p => p.IdUnidad == filtro.IdUnidad);
        if (filtro.Anio.HasValue) pagos = pagos.Where(p => p.Anio == filtro.Anio);
        if (filtro.Mes.HasValue) pagos = pagos.Where(p => p.Mes == filtro.Mes);
        if (!string.IsNullOrWhiteSpace(filtro.Estado)) pagos = pagos.Where(p => p.Estado == filtro.Estado);

        return await pagos.OrderByDescending(p => p.Anio).ThenByDescending(p => p.Mes).ThenBy(p => p.Unidad.Condominio.Nombre).ThenBy(p => p.Unidad.NumeroUnidad)
            .Select(p => PagoItemVM.Desde(p)).ToListAsync();
    }

    public async Task<PagoDetalleVM?> ObtenerAsync(int idPago) => await _context.Pagos.AsNoTracking()
        .Include(p => p.Unidad).ThenInclude(u => u.Condominio).Where(p => p.IdPago == idPago)
        .Select(p => PagoDetalleVM.Desde(p)).SingleOrDefaultAsync();

    public async Task<IReadOnlyList<SelectListItem>> ListarCondominiosAsync() => await _context.Condominios.AsNoTracking().OrderBy(c => c.Nombre)
        .Select(c => new SelectListItem(c.Nombre, c.IdCondominio.ToString())).ToListAsync();

    public async Task<IReadOnlyList<SelectListItem>> ListarUnidadesAsync(int? idCondominio = null, bool soloOcupadas = false)
    {
        var unidades = _context.Unidades.AsNoTracking().Include(u => u.Condominio).AsQueryable();
        if (idCondominio.HasValue) unidades = unidades.Where(u => u.IdCondominio == idCondominio);
        if (soloOcupadas) unidades = unidades.Where(u => u.UnidadPersonas.Any(up => up.TipoRelacion == "INQUILINO"));
        return await unidades.OrderBy(u => u.Condominio.Nombre).ThenBy(u => u.NumeroUnidad)
            .Select(u => new SelectListItem(u.Condominio.Nombre + " - Unidad " + u.NumeroUnidad, u.IdUnidad.ToString())).ToListAsync();
    }

    public async Task<ResultadoOperacion> CrearManualAsync(PagoCreateVM modelo)
    {
        if (!await UnidadOcupadaAsync(modelo.IdUnidad)) return ResultadoOperacion.Error("Solo se pueden generar expensas para unidades ocupadas.");
        if (await ExistePeriodoAsync(modelo.IdUnidad, modelo.Anio, modelo.Mes)) return ResultadoOperacion.Error("Ya existe una expensa para esta unidad y período.");
        var pago = new Pago { IdUnidad = modelo.IdUnidad, Anio = modelo.Anio, Mes = modelo.Mes, Monto = modelo.Monto, Concepto = modelo.Concepto.Trim(), FechaVencimiento = modelo.FechaVencimiento.Date, Estado = EstadoPago.Pendiente };
        _context.Pagos.Add(pago);
        try { await _context.SaveChangesAsync(); return ResultadoOperacion.Ok("La expensa fue creada correctamente."); }
        catch (DbUpdateException ex) when (EsDuplicado(ex)) { return ResultadoOperacion.Error("Ya existe una expensa para esta unidad y período."); }
    }

    public async Task<ResultadoOperacion> ActualizarAsync(PagoEditVM modelo)
    {
        var pago = await _context.Pagos.FindAsync(modelo.IdPago);
        if (pago is null) return ResultadoOperacion.Error("La expensa no existe.");
        if (pago.Estado == EstadoPago.Pagado) return ResultadoOperacion.Error("Una expensa pagada no puede modificarse.");
        pago.Concepto = modelo.Concepto.Trim(); pago.FechaVencimiento = modelo.FechaVencimiento.Date;
        await _context.SaveChangesAsync();
        return ResultadoOperacion.Ok("La expensa fue actualizada correctamente.");
    }

    public async Task<ResultadoOperacion> RegistrarPagoAsync(int idPago, DateTime fechaPago)
    {
        var pago = await _context.Pagos.FindAsync(idPago);
        if (pago is null) return ResultadoOperacion.Error("La expensa no existe.");
        if (pago.Estado == EstadoPago.Pagado) return ResultadoOperacion.Error("La expensa ya fue cancelada.");
        if (fechaPago == default) return ResultadoOperacion.Error("La fecha de pago es obligatoria.");
        pago.Estado = EstadoPago.Pagado; pago.FechaPago = fechaPago.Date;
        await _context.SaveChangesAsync();
        return ResultadoOperacion.Ok("El pago completo fue registrado correctamente.");
    }

    public async Task<ResultadoOperacion> EliminarAsync(int idPago)
    {
        var pago = await _context.Pagos.FindAsync(idPago);
        if (pago is null) return ResultadoOperacion.Error("La expensa no existe.");
        if (pago.Estado == EstadoPago.Pagado) return ResultadoOperacion.Error("No se puede eliminar una expensa pagada.");
        _context.Pagos.Remove(pago); await _context.SaveChangesAsync();
        return ResultadoOperacion.Ok("La expensa pendiente fue eliminada.");
    }

    public async Task<IReadOnlyList<SaldoUnidadVM>> ObtenerSaldosAsync(int? idCondominio)
    {
        var pagos = _context.Pagos.AsNoTracking().Include(p => p.Unidad).ThenInclude(u => u.Condominio).AsQueryable();
        if (idCondominio.HasValue) pagos = pagos.Where(p => p.Unidad.IdCondominio == idCondominio);
        return await pagos.GroupBy(p => new { p.IdUnidad, p.Unidad.NumeroUnidad, Condominio = p.Unidad.Condominio.Nombre })
            .Select(g => new SaldoUnidadVM { Condominio = g.Key.Condominio, Unidad = g.Key.NumeroUnidad, TotalFacturado = g.Sum(p => p.Monto), TotalCobrado = g.Where(p => p.Estado == EstadoPago.Pagado).Sum(p => (decimal?)p.Monto) ?? 0, TotalPendiente = g.Where(p => p.Estado == EstadoPago.Pendiente).Sum(p => (decimal?)p.Monto) ?? 0 }).ToListAsync();
    }

    public async Task<ResultadoGeneracionExpensas> GenerarMesActualAsync()
    {
        var hoy = DateTime.Today; var unidades = await _context.Unidades.Where(u => u.UnidadPersonas.Any(up => up.TipoRelacion == "INQUILINO")).ToListAsync();
        var existentes = await _context.Pagos.Where(p => p.Anio == hoy.Year && p.Mes == hoy.Month).Select(p => p.IdUnidad).ToListAsync();
        var nuevas = unidades.Where(u => !existentes.Contains(u.IdUnidad)).Select(u => new Pago { IdUnidad = u.IdUnidad, Anio = hoy.Year, Mes = hoy.Month, Monto = u.PagoMes, Concepto = $"Expensa {hoy:MM/yyyy}", FechaVencimiento = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(1).AddDays(-1), Estado = EstadoPago.Pendiente }).ToList();
        _context.Pagos.AddRange(nuevas);
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateException ex) when (EsDuplicado(ex)) { /* La restricción única mantiene la operación segura ante un reintento concurrente. */ }
        return new ResultadoGeneracionExpensas(hoy.Year, hoy.Month, nuevas.Count, unidades.Count - nuevas.Count);
    }

    private Task<bool> UnidadOcupadaAsync(int idUnidad) => _context.Unidades.AnyAsync(u => u.IdUnidad == idUnidad && u.UnidadPersonas.Any(up => up.TipoRelacion == "INQUILINO"));
    private Task<bool> ExistePeriodoAsync(int idUnidad, int anio, int mes) => _context.Pagos.AnyAsync(p => p.IdUnidad == idUnidad && p.Anio == anio && p.Mes == mes);
    private static bool EsDuplicado(DbUpdateException ex) => ex.GetBaseException() is SqlException { Number: 2601 or 2627 };
}
