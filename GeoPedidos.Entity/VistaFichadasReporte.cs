using System;
using System.Collections.Generic;

namespace GeoPedidos.Entity;

public partial class VistaFichadasReporte
{
    public DateTime? Fecha { get; set; }

    public DateTime? FechaOperativa { get; set; }

    public string? Motivo { get; set; }

    public int? Legajo { get; set; }

    public int? CodigoMotivo { get; set; }

    public DateTime? Hora { get; set; }

    public decimal? HorasNetas { get; set; }

    public string? ApellidoNombre { get; set; }

    public int? Sucursal { get; set; }

    public string? Horas { get; set; }
}
