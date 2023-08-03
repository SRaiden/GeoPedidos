using System;
using System.Collections.Generic;

namespace GeoPedidos.Entity;

public partial class FabricaPedidosDetalle
{
    public int Id { get; set; }

    public int? Codigo { get; set; }

    public int? Cantidad { get; set; }

    public decimal? Kilo { get; set; }

    public int? Entregado { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int? IdPedido { get; set; }
}
