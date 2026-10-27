using Gestion_Condominios.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gestion_Condominios.Services;

public interface IPagosService
{
    Task<IReadOnlyList<PagoItemVM>> BuscarAsync(PagoFiltro filtro);
    Task<PagoDetalleVM?> ObtenerAsync(int idPago);
    Task<IReadOnlyList<SelectListItem>> ListarCondominiosAsync();
    Task<IReadOnlyList<SelectListItem>> ListarUnidadesAsync(int? idCondominio = null, bool soloOcupadas = false);
    Task<ResultadoOperacion> CrearManualAsync(PagoCreateVM modelo);
    Task<ResultadoOperacion> ActualizarAsync(PagoEditVM modelo);
    Task<ResultadoOperacion> RegistrarPagoAsync(int idPago, DateTime fechaPago);
    Task<ResultadoOperacion> EliminarAsync(int idPago);
    Task<IReadOnlyList<SaldoUnidadVM>> ObtenerSaldosAsync(int? idCondominio);
}

public interface IGeneradorExpensasService
{
    Task<ResultadoGeneracionExpensas> GenerarMesActualAsync();
}

public sealed record ResultadoOperacion(bool Exito, string Mensaje)
{
    public static ResultadoOperacion Ok(string mensaje) => new(true, mensaje);
    public static ResultadoOperacion Error(string mensaje) => new(false, mensaje);
}

public sealed record ResultadoGeneracionExpensas(int Anio, int Mes, int Generadas, int Omitidas)
{
    public string Resumen => $"Se generaron {Generadas} expensa(s) para {Mes:D2}/{Anio}; {Omitidas} ya existían.";
}
