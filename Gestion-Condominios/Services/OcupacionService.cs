using Gestion_Condominios.Data;
using Gestion_Condominios.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Gestion_Condominios.Services;

public class OcupacionService : IOcupacionService
{
    private readonly CondominioDbContext _context;
    public OcupacionService(CondominioDbContext context) => _context = context;

    public async Task<IReadOnlyList<UnidadPersona>> ObtenerOcupantesAsync(int idUnidad) => await _context.UnidadesPersonas
        .AsNoTracking().Include(x => x.Persona).Where(x => x.IdUnidad == idUnidad)
        .OrderByDescending(x => x.TipoRelacion == "INQUILINO").ThenBy(x => x.Persona.Apellido).ToListAsync();

    public async Task<string> ObtenerEstadoAsync(int idUnidad) =>
        await _context.UnidadesPersonas.AnyAsync(x => x.IdUnidad == idUnidad) ? "OCUPADA" : "DISPONIBLE";

    public Task<ResultadoOcupacion> AsignarInquilinoAsync(int idUnidad, int idPersona, DateTime fechaInicio) =>
        EjecutarAsync(idUnidad, idPersona, fechaInicio, "INQUILINO", async ocupantes =>
        {
            if (ocupantes.Count != 0) return ResultadoOcupacion.Error("La unidad ya tiene ocupantes; use el cambio controlado de inquilino.");
            _context.UnidadesPersonas.Add(new UnidadPersona { IdUnidad = idUnidad, IdPersona = idPersona, TipoRelacion = "INQUILINO", FechaInicio = fechaInicio.Date });
            return ResultadoOcupacion.Ok("Inquilino asignado correctamente.");
        });

    public Task<ResultadoOcupacion> AgregarHabitanteAsync(int idUnidad, int idPersona, DateTime fechaInicio) =>
        EjecutarAsync(idUnidad, idPersona, fechaInicio, "HABITANTE", async ocupantes =>
        {
            if (!ocupantes.Any(x => x.TipoRelacion == "INQUILINO")) return ResultadoOcupacion.Error("No se puede agregar un habitante sin un inquilino responsable.");
            _context.UnidadesPersonas.Add(new UnidadPersona { IdUnidad = idUnidad, IdPersona = idPersona, TipoRelacion = "HABITANTE", FechaInicio = fechaInicio.Date });
            return ResultadoOcupacion.Ok("Habitante agregado correctamente.");
        });

    public async Task<ResultadoOcupacion> CambiarInquilinoAsync(int idUnidad, int idPersona, DateTime fechaInicio)
    {
        if (fechaInicio == default) return ResultadoOcupacion.Error("La fecha de inicio es obligatoria.");
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var unidad = await _context.Unidades.FindAsync(idUnidad);
            if (unidad is null) return ResultadoOcupacion.Error("La unidad no existe.");
            if (!await _context.Personas.AnyAsync(x => x.IdPersona == idPersona)) return ResultadoOcupacion.Error("La persona no existe.");
            var ocupantes = await _context.UnidadesPersonas.Where(x => x.IdUnidad == idUnidad).ToListAsync();
            var actual = ocupantes.SingleOrDefault(x => x.TipoRelacion == "INQUILINO");
            if (actual is null) return ResultadoOcupacion.Error("La unidad no tiene un inquilino que pueda reemplazarse.");
            if (actual.IdPersona == idPersona) return ResultadoOcupacion.Error("La persona indicada ya es el inquilino responsable.");
            var existente = ocupantes.SingleOrDefault(x => x.IdPersona == idPersona);
            _context.UnidadesPersonas.Remove(actual);
            // El índice filtrado sólo admite un inquilino. Se quita el actual antes de promover o crear
            // el nuevo, siempre dentro de la misma transacción serializable.
            await _context.SaveChangesAsync();
            if (existente is not null) { existente.TipoRelacion = "INQUILINO"; existente.FechaInicio = fechaInicio.Date; }
            else _context.UnidadesPersonas.Add(new UnidadPersona { IdUnidad = idUnidad, IdPersona = idPersona, TipoRelacion = "INQUILINO", FechaInicio = fechaInicio.Date });
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return ResultadoOcupacion.Ok("El inquilino responsable se cambió correctamente.");
        }
        catch (Exception ex) when (EsConflicto(ex)) { await transaction.RollbackAsync(); return ResultadoOcupacion.Error("La ocupación cambió simultáneamente. Intente nuevamente."); }
    }

    public async Task<ResultadoOcupacion> RetirarHabitanteAsync(int idUnidad, int idPersona)
    {
        var relacion = await _context.UnidadesPersonas.FindAsync(idUnidad, idPersona);
        if (relacion is null) return ResultadoOcupacion.Error("La relación de ocupación no existe.");
        if (relacion.TipoRelacion == "INQUILINO") return ResultadoOcupacion.Error("No puede retirar al inquilino desde esta operación. Use el cambio de inquilino o desocupe la unidad.");
        _context.UnidadesPersonas.Remove(relacion);
        await _context.SaveChangesAsync();
        return ResultadoOcupacion.Ok("Habitante retirado sin eliminar su ficha personal.");
    }

    public async Task<ResultadoOcupacion> DesocuparAsync(int idUnidad)
    {
        if (!await _context.Unidades.AnyAsync(x => x.IdUnidad == idUnidad)) return ResultadoOcupacion.Error("La unidad no existe.");
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var relaciones = await _context.UnidadesPersonas.Where(x => x.IdUnidad == idUnidad).ToListAsync();
        _context.UnidadesPersonas.RemoveRange(relaciones);
        await _context.SaveChangesAsync(); await transaction.CommitAsync();
        return ResultadoOcupacion.Ok("La unidad fue desocupada; las fichas de las personas se conservaron.");
    }

    private async Task<ResultadoOcupacion> EjecutarAsync(int idUnidad, int idPersona, DateTime fechaInicio, string tipo, Func<List<UnidadPersona>, Task<ResultadoOcupacion>> operacion)
    {
        if (fechaInicio == default) return ResultadoOcupacion.Error("La fecha de inicio es obligatoria.");
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            if (!await _context.Unidades.AnyAsync(x => x.IdUnidad == idUnidad)) return ResultadoOcupacion.Error("La unidad no existe.");
            if (!await _context.Personas.AnyAsync(x => x.IdPersona == idPersona)) return ResultadoOcupacion.Error("La persona no existe.");
            var ocupantes = await _context.UnidadesPersonas.Where(x => x.IdUnidad == idUnidad).ToListAsync();
            if (ocupantes.Any(x => x.IdPersona == idPersona)) return ResultadoOcupacion.Error("La persona ya está vinculada a esta unidad.");
            var resultado = await operacion(ocupantes);
            if (!resultado.Exitoso) return resultado;
            await _context.SaveChangesAsync(); await transaction.CommitAsync(); return resultado;
        }
        catch (Exception ex) when (EsConflicto(ex)) { await transaction.RollbackAsync(); return ResultadoOcupacion.Error("No se pudo completar la operación por un cambio simultáneo. Intente nuevamente."); }
    }

    private static bool EsConflicto(Exception ex) => ex is DbUpdateConcurrencyException || ex.GetBaseException() is SqlException { Number: 2601 or 2627 };
}
