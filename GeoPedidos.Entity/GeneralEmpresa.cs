using System;
using System.Collections.Generic;

namespace GeoPedidos.Entity;

public partial class GeneralEmpresa
{
    public int Id { get; set; }

    public string? NombreEmpresa { get; set; }

    public string? RazonSocial { get; set; }

    public string? Estado { get; set; }

    public bool? Confirmar { get; set; }

    public string? MailAvisoPedido { get; set; }

    public string? Alias { get; set; }

    public decimal? ColorFondo { get; set; }

    public decimal? ColorFuente { get; set; }

    public string? RgbFondo { get; set; }

    public string? RgbFuente { get; set; }
}
