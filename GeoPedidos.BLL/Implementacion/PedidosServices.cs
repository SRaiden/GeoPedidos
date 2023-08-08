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
        private readonly IGenericRepository<FabricaUsuario> _usuarioRepository;
        private readonly IGenericRepository<GeneralSucursales> _sucursalRepository;

        public PedidosServices(IGenericRepository<FabricaPedido> pedidosRepository, IGenericRepository<FabricaPedidosDetalle> pedidosDetalleRepository,  
                                IGenericRepository<FabricaGusto> gustosRepository, IGenericRepository<FabricaProducto> productosRepository,
                                IGenericRepository<FabricaInsumo> insumosRepository, IGenericRepository<FabricaPasteleria> pasteleriaRepository,
                                IGenericRepository<FabricaUsuario> usuarioRepository, IGenericRepository<GeneralSucursales> sucursalRepository)
        {
            _pedidosRepository = pedidosRepository;
            _gustosRepository = gustosRepository;
            _productosRepository = productosRepository;
            _insumosRepository = insumosRepository;
            _pasteleriaRepository = pasteleriaRepository;
            _pedidosDetalleRepository = pedidosDetalleRepository;
            _usuarioRepository = usuarioRepository;
            _sucursalRepository = sucursalRepository;
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

        public async Task<List<FabricaPedido>> ObtenerPedidos(int user, int idSucursalElegida, int idEmpresaElegida, string tipo, string fechaDesde, string fechaHasta)
        {
            IQueryable<FabricaUsuario> queryFabrica = await _usuarioRepository.Consultar();
            FabricaUsuario datoUsuario = queryFabrica.Where(d => d.Id == user).First();
            //------------------------------------------------------------------------------//

            IQueryable<FabricaPedido> query = await _pedidosRepository.Consultar(); // obtengo todos los pedidos

            fechaDesde = fechaDesde.Replace("-", "/");
            fechaHasta = fechaHasta.Replace("-", "/");

            DateTime FD = DateTime.ParseExact(fechaDesde, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime FH = DateTime.ParseExact(fechaHasta, "yyyy/MM/dd", CultureInfo.InvariantCulture);

            //List<FabricaPedido> fabricaPedido;
            List<FabricaPedido> listaTotal = new List<FabricaPedido>();

            if (tipo == "todos")
            {
                if (datoUsuario.IdSucursal == 0) // el user es admin o  superadmin??
                {
                    if (idSucursalElegida == 0)  // filtro ver todas las sucursales?
                    {
                        // LOGICA PARA OBTENER TODAS LAS SUCURSALES DE UNA EMPRESA
                        IQueryable<GeneralSucursales> querySucursal = await _sucursalRepository.Consultar();

                        List<GeneralSucursales> AllSucursal = null;
                        if (datoUsuario.IdEmpresa == 0)
                        {
                            AllSucursal = querySucursal.Where(d => d.EmpresaId == idEmpresaElegida).ToList(); //SI SOY SUPERADMIN
                        }
                        else {
                            AllSucursal = querySucursal.Where(d => d.EmpresaId == datoUsuario.IdEmpresa).ToList(); // SI SOY ADMIN DE SUCURSALES
                        }

                        // AHORA BUSCO TODOS LOS PEDIDOS DE LAS SUCURSALES ENCONTRADAS Y LAS ALMACENO EN UNA LISTA APARTE
                        List<FabricaPedido> fabricaPedidoTemp;
                        foreach (var ite in AllSucursal)
                        {
                            fabricaPedidoTemp = query.Where(v => v.IdSucursal == ite.Id).ToList();
                            foreach(var ite2 in fabricaPedidoTemp)
                            {
                                listaTotal.Add(ite2);
                            }
                        }

                    }
                    else // Filtro cualquier sucursal
                    {
                        listaTotal = query.Where(v => v.IdSucursal == idSucursalElegida).ToList();
                    }
                }
                else // es user
                {
                    listaTotal = query.Where(v => v.IdSucursal == idSucursalElegida && v.IdUsuario == user).ToList();
                }
            }
            else
            {
                if (datoUsuario.IdSucursal == 0) // el user es admin o  superadmin??
                {
                    if (idSucursalElegida == 0)  // filtro ver todas las sucursales?
                    {
                        // LOGICA PARA OBTENER TODAS LAS SUCURSALES DE UNA EMPRESA
                        IQueryable<GeneralSucursales> querySucursal = await _sucursalRepository.Consultar();

                        List<GeneralSucursales> AllSucursal = null;
                        if (datoUsuario.IdEmpresa == 0)
                        {
                            AllSucursal = querySucursal.Where(d => d.EmpresaId == idEmpresaElegida).ToList(); //SI SOY SUPERADMIN
                        }
                        else
                        {
                            AllSucursal = querySucursal.Where(d => d.EmpresaId == datoUsuario.IdEmpresa).ToList(); // SI SOY ADMIN DE SUCURSALES
                        }

                        // AHORA BUSCO TODOS LOS PEDIDOS DE LAS SUCURSALES ENCONTRADAS Y LAS ALMACENO EN UNA LISTA APARTE
                        List<FabricaPedido> fabricaPedidoTemp;
                        foreach (var ite in AllSucursal)
                        {
                            fabricaPedidoTemp = query.Where(v => v.IdSucursal == ite.Id && v.Tipo == tipo).ToList();
                            foreach (var ite2 in fabricaPedidoTemp)
                            {
                                listaTotal.Add(ite2);
                            }
                        }

                    }
                    else // Filtro cualquier sucursal
                    {
                        listaTotal = query.Where(v => v.IdSucursal == idSucursalElegida && v.Tipo == tipo).ToList();
                    }
                }
                else // es user
                {
                    listaTotal = query.Where(v => v.IdSucursal == idSucursalElegida && v.IdUsuario == user && v.Tipo == tipo).ToList();
                }
            }

            //AL NO SALIR LA QUERY CON EL WHERE FILTRANDO POR FECHA, ANALIZO MEDIANTE UN FOREACH Y ELIMINO AQUELLAS FILAS QUE NO CUMPLEN LA CONDICION
            //ESTO SE PODRIA OPTIMIZAR SI EL WHERE CREATED.VALUE.DATE ME TOMA CON EL RANGO DE FECHADESDE Y FECHAHASTA
            List<FabricaPedido> fabricaPedidoResultado = new List<FabricaPedido>();
            for (int i = 0; i < listaTotal.Count; i++)
            {
                DateTime creado = DateTime.Parse(DateTime.Parse(listaTotal[i].Created.ToString()).ToString("dd/MM/yyyy"));
                if (creado >= FD && creado <= FH)  
                {
                    fabricaPedidoResultado.Add(listaTotal[i]);
                }
            }

            return fabricaPedidoResultado;
        }

        public async Task<FabricaPedido> Crear(FabricaPedido entidad, List<FabricaPedidosDetalle> entidad_dos)
        {
            try
            {
                // OBTENER ULTIMO NUMERO DE PEDIDO DE LA SUCURSAL
                IQueryable<FabricaPedido> query = await _pedidosRepository.Consultar();

                string numPedido = "";
                try
                {
                    numPedido = query.Where(s => s.IdSucursal == entidad.IdSucursal).OrderByDescending(p => p.NumeroPedido).FirstOrDefault().NumeroPedido.ToString();
                    entidad.NumeroPedido = Int32.Parse(numPedido) + 1;
                }
                catch
                {
                    entidad.NumeroPedido = 1;
                }

                // CREAMOS LA CABEZERA
                FabricaPedido pedidoCreado = await _pedidosRepository.Crear(entidad);
                if (pedidoCreado.Id == 0)
                    throw new TaskCanceledException("No se pudo crear el pedido");

                // BUSQUEDA DE LA EMPRESA DE LA SUCURSAL QUE HACE EL PEDIDO
                IQueryable<GeneralSucursales> querySucursal = await _sucursalRepository.Consultar();
                GeneralSucursales datoSucursal = querySucursal.Where(d => d.Id == entidad.IdSucursal).First();

                // DEJO PREGRABADO LAS CONSULTAS
                IQueryable<FabricaGusto> queryGusto = await _gustosRepository.Consultar();
                IQueryable<FabricaInsumo> queryInsumo = await _insumosRepository.Consultar();
                IQueryable<FabricaProducto> queryProducto = await _productosRepository.Consultar();
                IQueryable<FabricaPasteleria> queryPasteleria = await _pasteleriaRepository.Consultar();


                // CREAMOS LOS DETALLES
                for (int i = 0; i < entidad_dos.Count; i++)
                {
                    entidad_dos[i].IdPedido = pedidoCreado.Id;

                    // CAMBIAR EL CODIGO POR EL ID, YA QUE SE RELACIONA POR EL ID INTERNO Y NO EL CODIGO
                    if (entidad.Tipo == "helado") {
                        FabricaGusto fg = queryGusto.Where(d => d.Codigo == entidad_dos[i].Codigo && d.IdEmpresa == datoSucursal.EmpresaId).First();
                        entidad_dos[i].Codigo = fg.Id;
                    } else if (entidad.Tipo == "insumo") {
                        FabricaInsumo fi = queryInsumo.Where(d => d.Codigo == entidad_dos[i].Codigo && d.IdEmpresa == datoSucursal.EmpresaId.ToString()).First();
                        entidad_dos[i].Codigo = fi.Id;
                    } else if (entidad.Tipo == "producto") {
                        FabricaProducto fp = queryProducto.Where(d => d.Codigo == entidad_dos[i].Codigo && d.IdEmpresa == datoSucursal.EmpresaId).First();
                        entidad_dos[i].Codigo = fp.Id;
                    }
                    else {
                        FabricaPasteleria fp = queryPasteleria.Where(d => d.Codigo == entidad_dos[i].Codigo && d.IdEmpresa == datoSucursal.EmpresaId).First();
                        entidad_dos[i].Codigo = fp.Id;
                    }
                    

                    // ultimo ID de detalle Pedido
                    IQueryable<FabricaPedidosDetalle> queryDetalle = await _pedidosDetalleRepository.Consultar();
                    string idDetallePedido = queryDetalle.OrderByDescending(p => p.Id).FirstOrDefault().Id.ToString();
                    entidad_dos[i].Id = Int32.Parse(idDetallePedido) + 1;

                    FabricaPedidosDetalle pedidoDetalleCreado = await _pedidosDetalleRepository.Crear(entidad_dos[i]);
                    if (pedidoDetalleCreado.Id == 0)
                        throw new TaskCanceledException("No se pudo crear el detalle del Pedido");
                }
               

                return pedidoCreado;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<List<FabricaPedidosDetalle>> ObtenerPedidos(int idPedido)
        {
            // Buscar el ID del pedido
            IQueryable<FabricaPedidosDetalle> query = await _pedidosDetalleRepository.Consultar();
            IQueryable<FabricaPedido> queryCabecera = await _pedidosRepository.Consultar();
                FabricaPedido datoPedido = queryCabecera.Where(p => p.Id == idPedido).First();
            // DEJO PREGRABADO LAS CONSULTAS
            IQueryable<FabricaGusto> queryGusto = await _gustosRepository.Consultar();
            IQueryable<FabricaInsumo> queryInsumo = await _insumosRepository.Consultar();
            IQueryable<FabricaProducto> queryProducto = await _productosRepository.Consultar();
            IQueryable<FabricaPasteleria> queryPasteleria = await _pasteleriaRepository.Consultar();

            List<FabricaPedidosDetalle> analizar = query.Where(d => d.IdPedido == idPedido).ToList();
            foreach (var ite in analizar) {
                // ACTUALIZAR SEGUN EL CODIGO
                if (datoPedido.Tipo == "helado")
                {
                    FabricaGusto gusto = queryGusto.Where(g => g.Id == ite.Codigo).First();
                    ite.Codigo = gusto.Codigo; 
                }
                else if (datoPedido.Tipo == "insumo")
                {
                    FabricaInsumo insumo = queryInsumo.Where(g => g.Id == ite.Codigo).First();
                    ite.Codigo = insumo.Codigo;
                }
                else if (datoPedido.Tipo == "producto")
                {
                    FabricaProducto producto = queryProducto.Where(g => g.Id == ite.Codigo).First();
                    ite.Codigo = producto.Codigo;
                }
                else
                {
                    FabricaPasteleria pasteleria = queryPasteleria.Where(g => g.Id == ite.Codigo).First();
                    ite.Codigo = pasteleria.Codigo;
                }
            }

            return analizar;
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


                // ---------------------------------------    AGREGAMOS LOS NUEVOS PEDIDOS
                // DEJO PREGRABADO LAS CONSULTAS
                // BUSQUEDA DE LA EMPRESA DE LA SUCURSAL QUE HACE EL PEDIDO
                IQueryable<GeneralSucursales> querySucursal = await _sucursalRepository.Consultar();
                GeneralSucursales datoSucursal = querySucursal.Where(d => d.Id == entidad.IdSucursal).First();
                IQueryable<FabricaGusto> queryGusto = await _gustosRepository.Consultar();
                IQueryable<FabricaInsumo> queryInsumo = await _insumosRepository.Consultar();
                IQueryable<FabricaProducto> queryProducto = await _productosRepository.Consultar();
                IQueryable<FabricaPasteleria> queryPasteleria = await _pasteleriaRepository.Consultar();
                for (int i = 0; i < entidad_dos.Count; i++)
                {
                    entidad_dos[i].IdPedido = pedidoCabeceraEncontrado.Id;

                    // CAMBIAR EL CODIGO POR EL ID, YA QUE SE RELACIONA POR EL ID INTERNO Y NO EL CODIGO
                    if (pedidoCabeceraEncontrado.Tipo == "helado")
                    {
                        FabricaGusto fg = queryGusto.Where(d => d.Codigo == entidad_dos[i].Codigo && d.IdEmpresa == datoSucursal.EmpresaId).First();
                        entidad_dos[i].Codigo = fg.Id;
                    }
                    else if (pedidoCabeceraEncontrado.Tipo == "insumo")
                    {
                        FabricaInsumo fi = queryInsumo.Where(d => d.Codigo == entidad_dos[i].Codigo && d.IdEmpresa == datoSucursal.EmpresaId.ToString()).First();
                        entidad_dos[i].Codigo = fi.Id;
                    }
                    else if (pedidoCabeceraEncontrado.Tipo == "producto")
                    {
                        FabricaProducto fp = queryProducto.Where(d => d.Codigo == entidad_dos[i].Codigo && d.IdEmpresa == datoSucursal.EmpresaId).First();
                        entidad_dos[i].Codigo = fp.Id;
                    }
                    else
                    {
                        FabricaPasteleria fp = queryPasteleria.Where(d => d.Codigo == entidad_dos[i].Codigo && d.IdEmpresa == datoSucursal.EmpresaId).First();
                        entidad_dos[i].Codigo = fp.Id;
                    }


                    // ultimo ID de detalle Pedido
                    IQueryable<FabricaPedidosDetalle> queryDetalle = await _pedidosDetalleRepository.Consultar();
                    string idDetallePedido = queryDetalle.OrderByDescending(p => p.Id).FirstOrDefault().Id.ToString();
                    entidad_dos[i].Id = Int32.Parse(idDetallePedido) + 1;

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
                FabricaGusto fg = query.Where(e => e.Id == codigoProducto && e.IdEmpresa == idEmpresa).FirstOrDefault();
                if (fg == null) 
                        throw new TaskCanceledException("Hay un producto que fue eliminado o no se pudo cargar");
                concat = fg.Nombre + "@" + fg.Categoria;
            }
            else if (tipoProducto == "producto")
            {
                IQueryable<FabricaProducto> query = await _productosRepository.Consultar();
                FabricaProducto fg = query.Where(e => e.Id == codigoProducto && e.IdEmpresa == idEmpresa).FirstOrDefault();
                if (fg == null)
                    throw new TaskCanceledException("Hay un producto que fue eliminado o no se pudo cargar");
                concat = fg.Nombre + "@" + fg.Categoria;
            }
            else if (tipoProducto == "insumo")
            {
                
                IQueryable<FabricaInsumo> query = await _insumosRepository.Consultar();
                FabricaInsumo fg = query.Where(e => e.Id == codigoProducto && e.IdEmpresa == idEmpresa.ToString()).FirstOrDefault();
                if (fg == null)
                    throw new TaskCanceledException("Hay un producto que fue eliminado o no se pudo cargar");
                concat = fg.Nombre + "@" + fg.Categoria;
            }
            else // PASTELERIA
            {
                IQueryable<FabricaPasteleria> query = await _pasteleriaRepository.Consultar();
                FabricaPasteleria fg = query.Where(e => e.Id == codigoProducto && e.IdEmpresa == idEmpresa).FirstOrDefault();
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
