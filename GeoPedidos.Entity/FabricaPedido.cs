using System;
using System.Collections.Generic;

namespace GeoPedidos.Entity;

public partial class FabricaPedido
{
    public int Id { get; set; }

    public string? Tipo { get; set; }

    public int? NumeroPedido { get; set; }

    public string? Estado { get; set; }

    public int? Cantidad { get; set; }

    public int? Remito { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public DateTime? FechaLeido { get; set; }

    public DateTime? FechaRemitido { get; set; }

    public DateTime? FechaConfirmado { get; set; }

    public DateTime? FechaAnulado { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdUsuario { get; set; }

    public string? Nota { get; set; }

    public bool? LeidoCaja { get; set; }
}
