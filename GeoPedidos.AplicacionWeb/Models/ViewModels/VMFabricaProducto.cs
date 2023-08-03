namespace GeoPedidos.AplicacionWeb.Models.ViewModels
{
    public class VMFabricaProducto
    {
        public int Id { get; set; }

        public int? Codigo { get; set; }

        public string? Nombre { get; set; }

        public string? Categoria { get; set; }

        public bool? Activo { get; set; }

        public DateTime? Created { get; set; }

        public int? IdEmpresa { get; set; }
    }
}
