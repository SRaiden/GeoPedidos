namespace GeoPedidos.AplicacionWeb.Models.ViewModels
{
    public class VMFabricaPedidoDetalle
    {
        public int Id { get; set; }

        public int? Codigo { get; set; }

        public int? Cantidad { get; set; }

        public decimal? Kilo { get; set; }

        public int? Entregado { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }

        public int? IdPedido { get; set; }

        public string? Comentario { get; set; }

        public string? FechaEntrega { get; set; }

        public string? Mensaje { get; set; }

    }
}
