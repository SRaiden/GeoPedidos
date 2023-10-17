namespace GeoPedidos.AplicacionWeb.Models.ViewModels
{
    public class VMPDFPedido
    {
        public string? NumeroPedido { get; set; }

        public string? Estado { get; set; }

        public List<VMPedido>? Pedido { get; set; }

    }
}
