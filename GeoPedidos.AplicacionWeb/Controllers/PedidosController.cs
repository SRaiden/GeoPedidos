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

        public PedidosController(IMapper mapper, IPedidosServices pedidosServices, ISucursalServices sucursalServices, IFabricaUsuariosServices usuariosServices)
        {
            _mapper = mapper;
            _pedidosServices = pedidosServices;
            _sucursalServices = sucursalServices;
            _usuariosServices = usuariosServices;
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
        public async Task<IActionResult> ObtenerPedido(int idSucursal, int numeroPedido)
        {
            List<VMFabricaPedidoDetalle> vmPedidoDetalle = _mapper.Map<List<VMFabricaPedidoDetalle>>(await _pedidosServices.ObtenerPedidos(idSucursal, numeroPedido));
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
                FabricaPedido pedidoEditado = await _pedidosServices.Editar(_mapper.Map<FabricaPedido>(vm), _mapper.Map<List<FabricaPedidosDetalle>>(vmpd),
                                                                            Int32.Parse(modelo[0].idSucursalEditar.ToString()) , Int32.Parse(modelo[0].numeroPedidoEditar.ToString()));
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


        private async Task<int> obtenerEmpresa(int idSucursal)
        {
            GeneralSucursales datoSucursal = await _sucursalServices.ObtenerDatosSucursal(idSucursal);
            return Int32.Parse(datoSucursal.EmpresaId.ToString());
        }
    }
}
