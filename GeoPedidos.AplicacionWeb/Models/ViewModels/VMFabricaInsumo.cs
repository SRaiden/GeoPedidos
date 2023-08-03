namespace GeoPedidos.AplicacionWeb.Models.ViewModels
{
    public class VMFabricaInsumo
    {
        public int Id { get; set; }

        public int? Codigo { get; set; }

        public string? Nombre { get; set; }

        public string? Categoria { get; set; }

        public bool? Activo { get; set; }

        public string? IdEmpresa { get; set; }
    }
}
