using System.ComponentModel;
using System.Globalization;

namespace Gestion_Condominios.Models
{
    public class Pago
    {
        public int IdPago { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaPago { get; set; }
        public string Estado { get; set; } = "PENDIENTE";
        public string Concepto { get; set; } = string.Empty;
        public int Anio { get; set; }
        public int Mes {  get; set; }
        public int IdUnidad { get; set; }
        public Unidad Unidad { get; set; } = null!;
    }
}
