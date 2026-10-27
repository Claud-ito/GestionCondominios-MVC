namespace Gestion_Condominios.ViewModels
{
    public class CondominioDeleteViewModel
    {
        public int IdCondominio { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CantidadUnidades { get; set; }
        public bool PuedeEliminar => CantidadUnidades == 0;
    }
}
