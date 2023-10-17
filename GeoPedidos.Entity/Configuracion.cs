using System;
using System.Collections.Generic;

namespace GeoPedidos.Entity;

public partial class Configuracion
{
    public int? Id { get; set; }
    public string? Recurso { get; set; }

    public string? Propiedad { get; set; }

    public string? Valor { get; set; }
}
