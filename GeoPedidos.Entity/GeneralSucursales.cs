using System;
using System.Collections.Generic;

namespace GeoPedidos.Entity;

public partial class GeneralSucursales
{
    public int Id { get; set; }

    public int? EmpresaId { get; set; }

    public string? NombreSucursal { get; set; }

    public bool? Activa { get; set; }

    public string? Alias { get; set; }

    public int? NumeroSucursal { get; set; }

    public int? ListaPreciosId { get; set; }

    public string? Localidad { get; set; }

    public string? Ciudad { get; set; }

    public string? Domicilio { get; set; }

    public decimal? CostoEnvio { get; set; }

    public string? CodigoArea { get; set; }

    public string? Telefono { get; set; }

    public string? Whatsapp { get; set; }

    public bool? Activo { get; set; }

    public int? HorarioId { get; set; }

    public DateTime? UltimoPago { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public int? CantidadConexionesVencidas { get; set; }

    public DateTime? FechaLimiteConexionesVencidas { get; set; }

    public bool? AvisoLicencia { get; set; }

    public bool? SistemaBloqueado { get; set; }

}
