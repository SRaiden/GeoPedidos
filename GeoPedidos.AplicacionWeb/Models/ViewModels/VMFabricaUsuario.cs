namespace GeoPedidos.AplicacionWeb.Models.ViewModels
{
    public class VMFabricaUsuario
    {
        public int Id { get; set; }

        public string? Nombre { get; set; }

        public string? Apellido { get; set; }

        public string? Email { get; set; }

        public string? NombreEmpresa { get; set; }

        public string? NombreSucursal { get; set; }

        public int? IdEmpresa { get; set; }

        public int? IdSucursal { get; set; }

        public int? Active { get; set; }

        public string? Rol { get; set; }

        public string? Contraseña { get; set; }

        public string? ContraseñaNueva { get; set; }

        public string? Created { get; set; }

        public string? Modified { get; set; }

        public int? OkLogin { get; set; }

    }
}
