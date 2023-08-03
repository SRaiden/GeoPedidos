using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeoPedidos.Entity;

namespace GeoPedidos.BLL.Interfaces
{
    public interface IFabricaUsuariosServices
    {
        Task<List<FabricaUsuario>> Lista();
        Task<int> UltimoId();
        Task<FabricaUsuario> ObtenerCredenciales(string correo, string clave); // consultar para su login
        Task<FabricaUsuario> ObtenerId(int IdUsuario);
        Task<bool> GuardarPerfil(FabricaUsuario entidad);
        Task<bool> CambiarClave(int IdUsuario, string claveActual, string claveNueva);
        Task<bool> RestablecerClave(string CorreoDestino, string URLPlantillaCorreo);

        // ------------------------------------- //

        Task<FabricaUsuario> Crear(FabricaUsuario entidad);
        Task<FabricaUsuario> Editar(FabricaUsuario entidad);
        Task<bool> Eliminar(int idUsuario);
        Task<bool> BajaLogica(int idUsuario);
        Task<FabricaUsuario> DatoUsuario(int idUsuario);
    }
}
