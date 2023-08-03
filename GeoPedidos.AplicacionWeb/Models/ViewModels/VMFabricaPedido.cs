namespace GeoPedidos.AplicacionWeb.Models.ViewModels
{
    public class VMFabricaPedido
    {
        public int Id { get; set; }

        public string? Tipo { get; set; }

        public int? NumeroPedido { get; set; }

        public string? Estado { get; set; }

        public int? Cantidad { get; set; }

        public int? Remito { get; set; }

        public int? IdEmpresa { get; set; }

        public int? IdSucursal { get; set; }

        public string? NombreSucursal { get; set; }

        public string? NombreUsuario { get; set; }

        public int? IdUsuario { get; set; }

        public string? FechaDesde { get; set; }

        public string? FechaHasta { get; set; }

        public string? Created { get; set; }


    }
}
