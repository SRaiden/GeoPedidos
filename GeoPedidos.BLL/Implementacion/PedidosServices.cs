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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace GeoPedidos.BLL.Implementacion
{
    public class PedidosServices : IPedidosServices
    {
        private readonly IGenericRepository<FabricaPedido> _pedidosRepository;
        private readonly IGenericRepository<FabricaPedidosDetalle> _pedidosDetalleRepository;
        private readonly IGenericRepository<FabricaGusto> _gustosRepository;
        private readonly IGenericRepository<FabricaProducto> _productosRepository;
        private readonly IGenericRepository<FabricaInsumo> _insumosRepository;
        private readonly IGenericRepository<FabricaPasteleria> _pasteleriaRepository;

        public PedidosServices(IGenericRepository<FabricaPedido> pedidosRepository, IGenericRepository<FabricaPedidosDetalle> pedidosDetalleRepository,  
                                IGenericRepository<FabricaGusto> gustosRepository, IGenericRepository<FabricaProducto> productosRepository,
                                IGenericRepository<FabricaInsumo> insumosRepository, IGenericRepository<FabricaPasteleria> pasteleriaRepository)
        {
            _pedidosRepository = pedidosRepository;
            _gustosRepository = gustosRepository;
            _productosRepository = productosRepository;
            _insumosRepository = insumosRepository;
            _pasteleriaRepository = pasteleriaRepository;
            _pedidosDetalleRepository = pedidosDetalleRepository;
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
                DateTime creado = DateTime.Parse(DateTime.Parse(fabricaPedido[i].Created.ToString()).ToString("dd/MM/yyyy"));
                if (!(creado >= FD && creado <= FH))  
                {
                    fabricaPedido.RemoveAt(i);
                }
                else
                {
                    var a = 0;
                }
            }

            return fabricaPedido;
        }

        public async Task<FabricaPedido> Crear(FabricaPedido entidad, List<FabricaPedidosDetalle> entidad_dos)
        {
            try
            {
                // OBTENER ULTIMO NUMERO DE PEDIDO DE LA SUCURSAL
                IQueryable<FabricaPedido> query = await _pedidosRepository.Consultar();
                string numPedido = query.Where(s => s.IdSucursal == entidad.IdSucursal).OrderByDescending(p => p.NumeroPedido).FirstOrDefault().NumeroPedido.ToString();
                entidad.NumeroPedido = Int32.Parse(numPedido) + 1;

                // CREAMOS LA CABEZERA
                FabricaPedido pedidoCreado = await _pedidosRepository.Crear(entidad);
                if (pedidoCreado.Id == 0)
                    throw new TaskCanceledException("No se pudo crear el pedido");

                // CREAMOS LOS DETALLES
                for (int i = 0; i < entidad_dos.Count; i++)
                {
                    entidad_dos[i].IdPedido = pedidoCreado.Id;
                    FabricaPedidosDetalle pedidoDetalleCreado = await _pedidosDetalleRepository.Crear(entidad_dos[i]);
                    if (pedidoDetalleCreado.Id == 0)
                        throw new TaskCanceledException("No se pudo crear el detalle del Pedido");
                }
               

                return pedidoCreado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<FabricaPedidosDetalle>> ObtenerPedidos(int idPedido)
        {
            // Buscar el ID del pedido
            IQueryable<FabricaPedidosDetalle> query = await _pedidosDetalleRepository.Consultar(); // obtengo todos los pedidos
            return query.Where(d => d.IdPedido == idPedido).ToList();
        }

        public async Task<FabricaPedido> Editar(FabricaPedido entidad, List<FabricaPedidosDetalle> entidad_dos, int idPedido)
        {
            try
            {
                // ACTUALIZAR CABECERA PEDIDO
                IQueryable<FabricaPedido> query = await _pedidosRepository.Consultar();
                FabricaPedido pedidoCabeceraEncontrado = query.Where(s => s.Id == idPedido).First();
                pedidoCabeceraEncontrado.Cantidad = entidad.Cantidad;
                pedidoCabeceraEncontrado.Modified = entidad.Created;
                pedidoCabeceraEncontrado.Estado = entidad.Estado;
                bool respuesta = await _pedidosRepository.Editar(pedidoCabeceraEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar el pedido");



                // ACTUALIZAR DETALLES DEL PEDIDO
                IQueryable<FabricaPedidosDetalle> query2 = await _pedidosDetalleRepository.Consultar();
                List<FabricaPedidosDetalle> pedidoDetalleEncontrado = query2.Where(s => s.IdPedido == idPedido).ToList();
                // BORRAMOS LOS REGISTROS QUE TENIA ANTES Y AGREGAMOS UNOS NUEVOS CON EL MISMO ID_PEDIDO
                for (int i = 0; i < pedidoDetalleEncontrado.Count; i++)
                {
                    bool respuestaDetalle = await _pedidosDetalleRepository.Eliminar(pedidoDetalleEncontrado[i]);
                }
                // AGREGAMOS LOS NUEVOS PEDIDOS
                for (int i = 0; i < entidad_dos.Count; i++)
                {
                    entidad_dos[i].IdPedido = pedidoCabeceraEncontrado.Id;
                    FabricaPedidosDetalle pedidoDetalleCreado = await _pedidosDetalleRepository.Crear(entidad_dos[i]);
                    if (pedidoDetalleCreado.Id == 0)
                        throw new TaskCanceledException("No se pudo crear el detalle del Pedido");
                }

                return pedidoCabeceraEncontrado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idPedido)
        {
            try
            {
                // BORRAMOS LOS DETALLES DEL PEDIDO
                IQueryable<FabricaPedidosDetalle> query = await _pedidosDetalleRepository.Consultar();
                List<FabricaPedidosDetalle> pedidoDetalleEncontrado = query.Where(s => s.IdPedido == idPedido).ToList();
                for (int i = 0; i < pedidoDetalleEncontrado.Count; i++)
                {
                    bool respuestaDetalle = await _pedidosDetalleRepository.Eliminar(pedidoDetalleEncontrado[i]);
                    if (!respuestaDetalle)
                        throw new TaskCanceledException("No se pudo borrar el detalle del pedido");
                }

                // BORRAMOS CABECERA
                FabricaPedido PedidoEncontrado = await _pedidosRepository.Obtener(c => c.Id == idPedido);
                if (PedidoEncontrado == null)
                    throw new TaskCanceledException("El pedido no existe");

                bool respuesta = await _pedidosRepository.Eliminar(PedidoEncontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<FabricaPedidosDetalle>> VerDetallePedido(int idPedido)
        {
            IQueryable<FabricaPedidosDetalle> query = await _pedidosDetalleRepository.Consultar();
            return query.Where(s => s.IdPedido == idPedido).ToList();
        }

        public async Task<string> ObtenerDatoProducto(int codigoProducto, string tipoProducto, int idEmpresa)
        {
            string concat;
            if (tipoProducto == "helado")
            {
                IQueryable<FabricaGusto> query = await _gustosRepository.Consultar();
                FabricaGusto fg = query.Where(e => e.Codigo == codigoProducto && e.IdEmpresa == idEmpresa).FirstOrDefault();
                if (fg == null)
                        throw new TaskCanceledException("Hay un producto que fue eliminado o no se pudo cargar");
                concat = fg.Nombre + "@" + fg.Categoria;
            }
            else if (tipoProducto == "producto")
            {
                IQueryable<FabricaProducto> query = await _productosRepository.Consultar();
                FabricaProducto fg = query.Where(e => e.Codigo == codigoProducto && e.IdEmpresa == idEmpresa).FirstOrDefault();
                if (fg == null)
                    throw new TaskCanceledException("Hay un producto que fue eliminado o no se pudo cargar");
                concat = fg.Nombre + "@" + fg.Categoria;
            }
            else if (tipoProducto == "insumo")
            {
                
                IQueryable<FabricaInsumo> query = await _insumosRepository.Consultar();
                FabricaInsumo fg = query.Where(e => e.Codigo == codigoProducto && e.IdEmpresa == idEmpresa.ToString()).FirstOrDefault();
                if (fg == null)
                    throw new TaskCanceledException("Hay un producto que fue eliminado o no se pudo cargar");
                concat = fg.Nombre + "@" + fg.Categoria;
            }
            else // PASTELERIA
            {
                IQueryable<FabricaPasteleria> query = await _pasteleriaRepository.Consultar();
                FabricaPasteleria fg = query.Where(e => e.Codigo == codigoProducto && e.IdEmpresa == idEmpresa).FirstOrDefault();
                if (fg == null)
                    throw new TaskCanceledException("Hay un producto que fue eliminado o no se pudo cargar");
                concat = fg.Nombre + "@" + fg.Categoria;
            }

            return concat;
        }

        public async Task<FabricaPedido> VerCabeceraPedido(int idPedido)
        {
            IQueryable<FabricaPedido> query = await _pedidosRepository.Consultar();
            return query.Where(s => s.Id == idPedido).First();
        }
    }
}
