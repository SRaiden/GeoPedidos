using System;
using System.Collections.Generic;

namespace GeoPedidos.Entity;

public partial class FabricaPasteleria
{
    public int Id { get; set; }

    public int? Codigo { get; set; }

    public string? Nombre { get; set; }

    public string? Categoria { get; set; }

    public bool? Activo { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int? IdEmpresa { get; set; }
}
