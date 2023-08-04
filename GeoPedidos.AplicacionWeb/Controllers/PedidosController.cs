using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using GeoPedidos.AplicacionWeb.Models.ViewModels;
using GeoPedidos.AplicacionWeb.Utilidades.Response;
using GeoPedidos.BLL.Interfaces;
using GeoPedidos.Entity;
using Microsoft.AspNetCore.Authorization;
using GeoPedidos.BLL.Implementacion;
using Newtonsoft.Json.Linq;

namespace GeoPedidos.AplicacionWeb.Controllers
{
    [Authorize]
    public class PedidosController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPedidosServices _pedidosServices;
        private readonly ISucursalServices _sucursalServices;
        private readonly IFabricaUsuariosServices _usuariosServices;
        private readonly IEmpresasServices _empresasServices;

        public PedidosController(IMapper mapper, IPedidosServices pedidosServices, ISucursalServices sucursalServices, 
                                IFabricaUsuariosServices usuariosServices, IEmpresasServices empresasServices)
        {
            _mapper = mapper;
            _pedidosServices = pedidosServices;
            _sucursalServices = sucursalServices;
            _usuariosServices = usuariosServices;
            _empresasServices = empresasServices;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Busqueda(int IdUsuario, int idSucursal, string tipo, string fechaDesde, string fechaHasta)
        {
            List<VMFabricaPedido> vmListaProductos = _mapper.Map<List<VMFabricaPedido>>(await _pedidosServices.ObtenerPedidos(IdUsuario, idSucursal, tipo.Replace("\\", "").Replace("\"", ""), fechaDesde.Replace("\\", "").Replace("\"", ""), fechaHasta.Replace("\\", "").Replace("\"", "")));
            foreach(var i in vmListaProductos)
            {
                i.NombreSucursal = await _sucursalServices.ObtenerNombreSucursal(Int32.Parse(i.IdSucursal.ToString()));
                FabricaUsuario fu = await _usuariosServices.DatoUsuario(Int32.Parse(i.IdUsuario.ToString()));
                i.NombreUsuario = fu.Nombre;
            }

            return StatusCode(StatusCodes.Status200OK, new { data = vmListaProductos });
        }

        [HttpGet]
        public async Task<IActionResult> CargarHelado(int idSucursal = 1)
        {
            // obtener empresa
            int idEmpresa = await obtenerEmpresa(idSucursal);

            List<VMListaProductos> vmLista = _mapper.Map<List<VMListaProductos>>(await _pedidosServices.ObtenerHelados(idEmpresa));
            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }

        [HttpGet]
        public async Task<IActionResult> CargarProducto(int idSucursal)
        {
            int idEmpresa = await obtenerEmpresa(idSucursal);

            List<VMListaProductos> vmLista = _mapper.Map<List<VMListaProductos>>(await _pedidosServices.ObtenerProductos(idEmpresa));
            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }

        [HttpGet]
        public async Task<IActionResult> CargarInsumo(int idSucursal)
        {
            int idEmpresa = await obtenerEmpresa(idSucursal);

            List<VMListaProductos> vmLista = _mapper.Map<List<VMListaProductos>>(await _pedidosServices.ObtenerInsumos(idEmpresa));
            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }

        [HttpGet]
        public async Task<IActionResult> CargarPasteleria(int idSucursal)
        {
            int idEmpresa = await obtenerEmpresa(idSucursal);

            List<VMListaProductos> vmLista = _mapper.Map<List<VMListaProductos>>(await _pedidosServices.ObtenerPastelerias(idEmpresa));
            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPedido(int idPedido)
        {
            List<VMFabricaPedidoDetalle> vmPedidoDetalle = _mapper.Map<List<VMFabricaPedidoDetalle>>(await _pedidosServices.ObtenerPedidos(idPedido));
            return StatusCode(StatusCodes.Status200OK, vmPedidoDetalle);
        }

        //------------------------------------ ABM ---------------------------------------------------// 

        [HttpPost]
        public async Task<IActionResult> GuardarPedido([FromBody] List<VMPedido> modelo)
        {
            GenericResponse<VMFabricaPedido> gResponse = new GenericResponse<VMFabricaPedido>();

            try
            {
                // SEPARAR
                VMFabricaPedido vm = new VMFabricaPedido();
                vm.Tipo = modelo[0].TipoCabecera;
                vm.Estado = modelo[0].EstadoCabecera;
                vm.Cantidad = modelo[0].CantidadCabecera;
                vm.Created = modelo[0].CreatedDetalle;
                vm.Remito = 0;
                vm.IdUsuario = 1; // Luego se modifica con los permisos para saber que usuario lo registro
                vm.IdSucursal = 1; // Luego se modifica con los permisos para saber que sucursal lo registro

                List<VMFabricaPedidoDetalle> vmpd = new List<VMFabricaPedidoDetalle>();
                foreach(var i in modelo)
                {
                    VMFabricaPedidoDetalle vms = new VMFabricaPedidoDetalle();
                    vms.Codigo = i.CodigoDetalle;
                    vms.Cantidad = i.CantidadDetalle;
                    vms.Created = DateTime.Parse(i.CreatedDetalle.ToString());
                    vms.Kilo = 0;
                    vms.Entregado = 0;
                    vmpd.Add(vms);
                }

                //// CREAR PEDIDO
                FabricaPedido pedidoCreado = await _pedidosServices.Crear(_mapper.Map<FabricaPedido>(vm), _mapper.Map<List<FabricaPedidosDetalle>>(vmpd));
                vm = _mapper.Map<VMFabricaPedido>(pedidoCreado);

                //// Resultado
                gResponse.Estado = true;
                gResponse.Mensaje = "El " + vm.Tipo + " fue creado";
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> EditarPedido([FromBody] List<VMPedido> modelo)
        {
            GenericResponse<VMFabricaPedido> gResponse = new GenericResponse<VMFabricaPedido>();

            try
            {
                // SEPARAR
                VMFabricaPedido vm = new VMFabricaPedido();
                vm.Tipo = modelo[0].TipoCabecera;
                vm.Estado = modelo[0].EstadoCabecera;
                vm.Cantidad = modelo[0].CantidadCabecera;
                vm.Created = modelo[0].CreatedDetalle;
                vm.Remito = 0;
                vm.IdUsuario = 1; // Luego se modifica con los permisos para saber que usuario lo registro
                vm.IdSucursal = 1; // Luego se modifica con los permisos para saber que sucursal lo registro

                List<VMFabricaPedidoDetalle> vmpd = new List<VMFabricaPedidoDetalle>();
                foreach (var i in modelo)
                {
                    VMFabricaPedidoDetalle vms = new VMFabricaPedidoDetalle();
                    vms.Codigo = i.CodigoDetalle;
                    vms.Cantidad = i.CantidadDetalle;
                    vms.Created = DateTime.Parse(i.CreatedDetalle.ToString());
                    vms.Kilo = 0;
                    vms.Entregado = 0;
                    vmpd.Add(vms);
                }

                //// EDITAR PEDIDO
                FabricaPedido pedidoEditado = await _pedidosServices.Editar(_mapper.Map<FabricaPedido>(vm), _mapper.Map<List<FabricaPedidosDetalle>>(vmpd), Int32.Parse(modelo[0].idPedido.ToString()));
                vm = _mapper.Map<VMFabricaPedido>(pedidoEditado);

                //// Resultado
                gResponse.Estado = true;
                gResponse.Mensaje = "El " + vm.Tipo + " fue editado";
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int idPedido)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _pedidosServices.Eliminar(idPedido);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> VerDetallesPedido(int idPedido, string tipoPedido)
        {
            // saber que empresa pertenece la sucursal que pidio el producto
            VMFabricaPedido pedidoCabecera = _mapper.Map<VMFabricaPedido>(await _pedidosServices.VerCabeceraPedido(idPedido));
            VMGeneralSucursales datoSucursal = _mapper.Map<VMGeneralSucursales>(await _empresasServices.ObtenerDatoEmpresa(Int32.Parse(pedidoCabecera.IdSucursal.ToString())));

            // obtener codigo y cantidad
            List<VMFabricaPedidoDetalle> vmLista = _mapper.Map<List<VMFabricaPedidoDetalle>>(await _pedidosServices.VerDetallePedido(idPedido));

            // obtener nombre y categoria - al estar en otras tablas, obtenemos los resultado
            List<VMPedido> vmPedido = new List<VMPedido>();
            for(int a=0; a<vmLista.Count; a++)
            {
                VMPedido vm = new VMPedido();
                string resultado = await _pedidosServices.ObtenerDatoProducto(Int32.Parse(vmLista[a].Codigo.ToString()), tipoPedido.Replace("\\", "").Replace("\"", "").ToLower(), Int32.Parse(datoSucursal.EmpresaId.ToString()));
                string[] separar = resultado.Split('@');

                vm.CodigoDetalle = vmLista[a].Codigo;
                vm.CantidadDetalle = vmLista[a].Cantidad;
                vm.DescripcionDetalle = separar[0];
                vm.CategoriaDetalle = separar[1];
                vmPedido.Add(vm);
            }

            // enviar
            return StatusCode(StatusCodes.Status200OK, new { data = vmPedido });
        }

        private async Task<int> obtenerEmpresa(int idSucursal)
        {
            GeneralSucursales datoSucursal = await _sucursalServices.ObtenerDatosSucursal(idSucursal);
            return Int32.Parse(datoSucursal.EmpresaId.ToString());
        }
    }
}
