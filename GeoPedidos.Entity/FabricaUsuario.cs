using System;
using System.Collections.Generic;

namespace GeoPedidos.Entity;

public partial class FabricaUsuario
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public string? Email { get; set; }

    public string? Contraseña { get; set; }

    public string? Rol { get; set; }

    public bool? Active { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public int? IdEmpresa { get; set; }

    public int? IdSucursal { get; set; }

    public bool? OkLogin { get; set; }
}
