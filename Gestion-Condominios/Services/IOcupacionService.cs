using Gestion_Condominios.Models;

namespace Gestion_Condominios.Services;

public interface IOcupacionService
{
    Task<IReadOnlyList<UnidadPersona>> ObtenerOcupantesAsync(int idUnidad);
    Task<ResultadoOcupacion> AsignarInquilinoAsync(int idUnidad, int idPersona, DateTime fechaInicio);
    Task<ResultadoOcupacion> AgregarHabitanteAsync(int idUnidad, int idPersona, DateTime fechaInicio);
    Task<ResultadoOcupacion> CambiarInquilinoAsync(int idUnidad, int idPersona, DateTime fechaInicio);
    Task<ResultadoOcupacion> RetirarHabitanteAsync(int idUnidad, int idPersona);
    Task<ResultadoOcupacion> DesocuparAsync(int idUnidad);
    Task<string> ObtenerEstadoAsync(int idUnidad);
}

public sealed record ResultadoOcupacion(bool Exitoso, string Mensaje)
{
    public static ResultadoOcupacion Ok(string mensaje) => new(true, mensaje);
    public static ResultadoOcupacion Error(string mensaje) => new(false, mensaje);
}
