namespace GeoPedidos.AplicacionWeb.Models.ViewModels
{
    public class VMPedido
    {
        public string? TipoCabecera { get; set; }

        public string? EstadoCabecera { get; set; }

        public int? CantidadCabecera { get; set; }

        public string? CreatedCabecera { get; set; }

        public string? entregaCabecera { get; set; }

        //-------------------------------- DETALLE ------------------------------------ //

        public int? CodigoDetalle { get; set; }

        public int? CantidadDetalle { get; set; }

        public string? DescripcionDetalle { get; set; }

        public string? CategoriaDetalle { get; set; }

        public string? CreatedDetalle { get; set; }

        //-------------------------------- EDITAR ------------------------------------ //

        public int? idPedido { get; set; }

        public string? Comentario { get; set; }
    }
}
