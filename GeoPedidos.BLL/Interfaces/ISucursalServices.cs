using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeoPedidos.Entity;

namespace GeoPedidos.BLL.Interfaces
{
    public interface ISucursalServices
    {
        Task<List<GeneralSucursales>> Lista(int idEmpresa);

        Task<GeneralSucursales> ObtenerDatosSucursal(int id);

        Task<string> ObtenerNombreSucursal(int id);
    }
}
