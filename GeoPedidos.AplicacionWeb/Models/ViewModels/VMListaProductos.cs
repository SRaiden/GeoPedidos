namespace GeoPedidos.AplicacionWeb.Models.ViewModels
{
    public class VMListaProductos
    {
        public int Codigo { get; set; }

        public string? Nombre { get; set; }

        public string? Categoria { get; set; }

        public string? Cantidad { get; set; }

        public string? Pendiente { get; set; }

        public int? Activo { get; set; }

    }
}
