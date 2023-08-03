﻿namespace GeoPedidos.AplicacionWeb.Models.ViewModels
{
    public class VMPedido
    {
        public string? TipoCabecera { get; set; }

        public string? EstadoCabecera { get; set; }

        public int? CantidadCabecera { get; set; }

        public string? CreatedCabecera { get; set; }

        //-------------------------------- DETALLE ------------------------------------ //

        public int? CodigoDetalle { get; set; }

        public int? CantidadDetalle { get; set; }

        public string? CreatedDetalle { get; set; }

        //-------------------------------- EDITAR ------------------------------------ //

        public int? numeroPedidoEditar { get; set; }
        public int? idSucursalEditar { get; set; }
    }
}