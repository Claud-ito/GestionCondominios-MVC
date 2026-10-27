namespace Gestion_Condominios.ViewModels
{
    public class InicioViewModel
    {
        public int TotalCondominios { get; set; }
        public int TotalUnidades { get; set; }
        public int UnidadesOcupadas { get; set; }
        public int UnidadesDisponibles => TotalUnidades - UnidadesOcupadas;
        public int ExpensasPendientes { get; set; }
        public int ExpensasVencidas { get; set; }
        public decimal MontoPendiente { get; set; }
        public int VisitasAbiertas { get; set; }
    }
}
