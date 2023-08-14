namespace GeoPedidos.AplicacionWeb.Models.ViewModels
{
    public class VMBusqueda
    {
        public int? IdUsuario { get; set; }

        public int? idSucursal { get; set; }

        public int? idEmpresa { get; set; }

        public string? tipo { get; set; }

        public string? fechaDesde { get; set; }

        public string? fechaHasta { get; set; }

    }
}
