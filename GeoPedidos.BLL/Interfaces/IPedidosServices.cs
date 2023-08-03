﻿using GeoPedidos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPedidos.BLL.Interfaces
{
    public interface IPedidosServices
    {
        Task<List<FabricaPedido>> ObtenerPedidos(int iduser, int idSucursal, string tipo, string fechaDesde, string fechaHasta);

        Task<List<FabricaGusto>> ObtenerHelados(int idEmpresa);
        Task<List<FabricaProducto>> ObtenerProductos(int idEmpresa);
        Task<List<FabricaInsumo>> ObtenerInsumos(int idEmpresa);
        Task<List<FabricaPasteleria>> ObtenerPastelerias(int idEmpresa);
    }
}