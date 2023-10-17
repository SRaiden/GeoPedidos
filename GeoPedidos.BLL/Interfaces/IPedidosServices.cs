using GeoPedidos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPedidos.BLL.Interfaces
{
    public interface IPedidosServices
    {
        Task<List<FabricaPedido>> ObtenerPedidos(int iduser, int idSucursal, int idEmpresa, string tipo, string fechaDesde, string fechaHasta);
        //------------------------ AGREGAR -----------------------------//
        Task<List<FabricaGusto>> ObtenerHelados(int idEmpresa);
        Task<List<FabricaProducto>> ObtenerProductos(int idEmpresa);
        Task<List<FabricaInsumo>> ObtenerInsumos(int idEmpresa);
        Task<List<FabricaPasteleria>> ObtenerPastelerias(int idEmpresa);
        //------------------------ / AGREGAR -----------------------------//
        //------------------------ EDITAR -----------------------------//
        Task<FabricaGusto> ObtenerUnHelado(int codigo);
        Task<FabricaProducto> ObtenerUnProducto(int codigo);
        Task<FabricaInsumo> ObtenerUnInsumo(int codigo);
        Task<FabricaPasteleria> ObtenerUnPasteleria(int codigo);
        //--//
        Task<FabricaPedido> Crear(FabricaPedido entidad, List<FabricaPedidosDetalle> entidad_dos);
        Task<FabricaPedido> Editar(FabricaPedido entidad, List<FabricaPedidosDetalle> entidad_dos, int idPedido);
        Task<List<FabricaPedidosDetalle>> VerDetallePedido(int idPedido);
        Task<List<FabricaPedidosDetalle>> ObtenerCodigoRealProducto(int idPedido);
        Task<string> ObtenerNombreCategoriaProducto(int codigoProducto, string tipoProducto, int idEmpresa);
        Task<FabricaPedido> VerCabeceraPedido(int idPedido);
        Task<bool> Eliminar(int idPedido);
    }
}
