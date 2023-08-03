using System;
using System.Collections.Generic;

namespace GeoPedidos.Entity;

public partial class FabricaPedidosRemito
{
    public int Id { get; set; }

    public int? Codigo { get; set; }

    public decimal? Kilos { get; set; }

    public string? CodBarra { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int? IdPedido { get; set; }
}
