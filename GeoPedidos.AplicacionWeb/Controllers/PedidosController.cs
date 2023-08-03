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


        private async Task<int> obtenerEmpresa(int idSucursal)
        {
            GeneralSucursales datoSucursal = await _sucursalServices.ObtenerDatosSucursal(idSucursal);
            return Int32.Parse(datoSucursal.EmpresaId.ToString());
        }
    }
}
