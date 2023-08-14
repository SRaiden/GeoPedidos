using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeoPedidos.Entity;

namespace GeoPedidos.BLL.Interfaces
{
    public interface IEmpresasServices
    {
        Task<List<GeneralEmpresa>> Lista();
        Task<int> UltimoId();
        Task<GeneralSucursales> ObtenerDatoSucursal(int idSucursal);
        Task<GeneralEmpresa> ObtenerDatoEmpresa(int idEmpresa);
        Task<string> ObtenerNombreEmpresa(int id);
        Task<GeneralEmpresa> Crear(GeneralEmpresa entidad);
        Task<GeneralEmpresa> Editar(GeneralEmpresa entidad);
        Task<bool> Eliminar(int idEmpresa);
        Task<bool> BajaLogica(int idEmpresa);
    }
}
