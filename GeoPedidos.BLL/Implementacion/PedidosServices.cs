using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using GeoPedidos.BLL.Interfaces;
using GeoPedidos.DAL.Interfaces;
using GeoPedidos.Entity;
using System.Collections;


namespace GeoPedidos.BLL.Implementacion
{
    public class PedidosServices : IPedidosServices
    {
        private readonly IGenericRepository<FabricaPedido> _pedidosRepository;
        private readonly IGenericRepository<FabricaGusto> _gustosRepository;
        private readonly IGenericRepository<FabricaProducto> _productosRepository;
        private readonly IGenericRepository<FabricaInsumo> _insumosRepository;
        private readonly IGenericRepository<FabricaPasteleria> _pasteleriaRepository;

        public PedidosServices(IGenericRepository<FabricaPedido> pedidosRepository, IGenericRepository<FabricaGusto> gustosRepository, 
                                IGenericRepository<FabricaProducto> productosRepository, IGenericRepository<FabricaInsumo> insumosRepository, 
                                IGenericRepository<FabricaPasteleria> pasteleriaRepository)
        {
            _pedidosRepository = pedidosRepository;
            _gustosRepository = gustosRepository;
            _productosRepository = productosRepository;
            _insumosRepository = insumosRepository;
            _pasteleriaRepository = pasteleriaRepository;
        }

        public async Task<List<FabricaGusto>> ObtenerHelados(int idEmpresa)
        {
            IQueryable<FabricaGusto> query = await _gustosRepository.Consultar();
            return query.Where(e => e.IdEmpresa == idEmpresa).ToList();
        }

        public async Task<List<FabricaInsumo>> ObtenerInsumos(int idEmpresa)
        {
            IQueryable<FabricaInsumo> query = await _insumosRepository.Consultar();
            return query.Where(e => e.IdEmpresa == idEmpresa.ToString()).ToList();
        }

        public async Task<List<FabricaPasteleria>> ObtenerPastelerias(int idEmpresa)
        {
            IQueryable<FabricaPasteleria> query = await _pasteleriaRepository.Consultar();
            return query.Where(e => e.IdEmpresa == idEmpresa).ToList();
        }

        public async Task<List<FabricaProducto>> ObtenerProductos(int idEmpresa)
        {
            IQueryable<FabricaProducto> query = await _productosRepository.Consultar();
            return query.Where(e => e.IdEmpresa == idEmpresa).ToList();
        }

        //------------------------------------------------------------------------------------------//

        public async Task<List<FabricaPedido>> ObtenerPedidos(int user, int idSucursal, string tipo, string fechaDesde, string fechaHasta)
        {
            IQueryable<FabricaPedido> query = await _pedidosRepository.Consultar(); // obtengo todos los pedidos

            fechaDesde = fechaDesde.Replace("-", "/");
            fechaHasta = fechaHasta.Replace("-", "/");

            DateTime FD = DateTime.ParseExact(fechaDesde, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime FH = DateTime.ParseExact(fechaHasta, "yyyy/MM/dd", CultureInfo.InvariantCulture);

            List<FabricaPedido> fabricaPedido;
            if (tipo == "todos")
            {
                if (idSucursal == 0 && user == 0) fabricaPedido = query.ToList(); // si logeo el admin y filtro todas las sucursales
                else if (user == 0) fabricaPedido = query.Where(v => v.IdSucursal == idSucursal).ToList(); // si logueo el admin y filtro cualquier sucursal
                else fabricaPedido = query.Where(v => v.IdSucursal == idSucursal && v.IdUsuario == user).ToList(); // solo users
            }
            else
            {
                if (idSucursal == 0 && user == 0) fabricaPedido = query.Where(v => v.Tipo == tipo).ToList(); // si logeo el admin y filtro todas las sucursales
                else if (user == 0) fabricaPedido = query.Where(v => v.Tipo == tipo && v.IdSucursal == idSucursal).ToList(); // si logueo el admin y filtro cualquier sucursal
                else fabricaPedido = query.Where(v => v.Tipo == tipo && v.IdSucursal == idSucursal && v.IdUsuario == user).ToList(); // solo users
            }

            //AL NO SALIR LA QUERY CON EL WHERE FILTRANDO POR FECHA, ANALIZO MEDIANTE UN FOREACH Y ELIMINO AQUELLAS FILAS QUE NO CUMPLEN LA CONDICION
            //ESTO SE PODRIA OPTIMIZAR SI EL WHERE CREATED.VALUE.DATE ME TOMA CON EL RANGO DE FECHADESDE Y FECHAHASTA
            for(int i = 0; i < fabricaPedido.Count; i++)
            {
                if (fabricaPedido[i].Created < FD || fabricaPedido[i].Created > FH)  
                {
                    fabricaPedido.RemoveAt(i);
                }
            }

            return fabricaPedido;
        }

    }
}
