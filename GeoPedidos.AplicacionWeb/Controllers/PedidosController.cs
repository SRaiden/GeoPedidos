using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using GeoPedidos.AplicacionWeb.Models.ViewModels;
using GeoPedidos.AplicacionWeb.Utilidades.Response;
using GeoPedidos.BLL.Interfaces;
using GeoPedidos.Entity;
using Microsoft.AspNetCore.Authorization;
using GeoPedidos.BLL.Implementacion;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

using DinkToPdf;
using DinkToPdf.Contracts;

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
        private readonly IConverter _converter;

        public PedidosController(IMapper mapper, IPedidosServices pedidosServices, ISucursalServices sucursalServices, 
                                IFabricaUsuariosServices usuariosServices, IEmpresasServices empresasServices, IConverter converter)
        {
            _mapper = mapper;
            _pedidosServices = pedidosServices;
            _sucursalServices = sucursalServices;
            _usuariosServices = usuariosServices;
            _empresasServices = empresasServices;
            _converter = converter;
        }

        public async Task<IActionResult> Index()
        {
            // PARA OPCIONES DE QUE MOSTRAR EN EL LOGIN
            ClaimsPrincipal claimPrin = HttpContext.User;
            string idUser = "";
            string nombreUser = "";
            string IdSucursal = "";
            string idEmpresa = "";
            string rol = "";

            if (claimPrin.Identity.IsAuthenticated) // se logeo??
            { 

                // OBTENGO ID USER AL LOGEAR POR MEDIO DEL CLAIMPS
                idUser = claimPrin.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                // OBTENGO SU NOMBRE
                nombreUser = claimPrin.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();

                // OBTENGO IDSUCURSAL
                IdSucursal = claimPrin.Claims.Where(c => c.Type == ClaimTypes.Surname).Select(c => c.Value).SingleOrDefault();

                // OBTENGO ROL (0 -> ADMIN, 1 -> USER)
                rol = claimPrin.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();

                // Busco el IDEMPRESA (ESTO ES PARA SUPERADMIN)
                FabricaUsuario datoUsuario = await _usuariosServices.DatoUsuario(Int32.Parse(idUser));
                idEmpresa = datoUsuario.IdEmpresa.ToString();
            }

            ViewBag.Elegir = false;
            ViewBag.user = idUser;
            ViewBag.nombreUser = nombreUser;
            ViewBag.idSucursalLogin = IdSucursal;
            ViewBag.idEmpresaLogin = idEmpresa;
            ViewBag.rolLogin = rol;

            return View();
        }

        [HttpPost] // CODIGO TEMPORAL HASTA VER QUE ONDA QUE EL SERVIDOR NO EJECUTA EL OTRO METODO DE BUSQUEDA POR MEDIO DE UN DATABLE
        public async Task<IActionResult> Index(string userLogin, string cboEmpresas, string cboSucursales, string txtDesde, string txtHasta)
        {
            string rbReporte = Request.Form["rbReporte"];
            //----------------------------------------------------//

            List<VMFabricaPedido> vmListaProductos = _mapper.Map<List<VMFabricaPedido>>(await _pedidosServices.ObtenerPedidos(
                                                    Int32.Parse(userLogin.ToString()), Int32.Parse(cboSucursales.ToString()), Int32.Parse(cboEmpresas.ToString()),
                                                    rbReporte, txtDesde, txtHasta));

            foreach (var i in vmListaProductos)
            {
                i.NombreSucursal = await _sucursalServices.ObtenerNombreSucursal(Int32.Parse(i.IdSucursal.ToString()));
                FabricaUsuario fu = await _usuariosServices.DatoUsuario(Int32.Parse(i.IdUsuario.ToString()));
                i.NombreUsuario = fu.Nombre;
            }

            ViewBag.elementosEncontrados = vmListaProductos;

            // PARA OPCIONES DE QUE MOSTRAR EN EL LOGIN
            ClaimsPrincipal claimPrin = HttpContext.User;
            string idUser = "";
            string nombreUser = "";
            string IdSucursal = "";
            string idEmpresa = "";
            string rol = "";

            if (claimPrin.Identity.IsAuthenticated) // se logeo??
            {

                // OBTENGO ID USER AL LOGEAR POR MEDIO DEL CLAIMPS
                idUser = claimPrin.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                // OBTENGO SU NOMBRE
                nombreUser = claimPrin.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();

                // OBTENGO IDSUCURSAL
                IdSucursal = claimPrin.Claims.Where(c => c.Type == ClaimTypes.Surname).Select(c => c.Value).SingleOrDefault();

                // OBTENGO ROL (0 -> ADMIN, 1 -> USER)
                rol = claimPrin.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();

                // Busco el IDEMPRESA (ESTO ES PARA SUPERADMIN)
                FabricaUsuario datoUsuario = await _usuariosServices.DatoUsuario(Int32.Parse(idUser));
                idEmpresa = datoUsuario.IdEmpresa.ToString();
            }

            ViewBag.user = idUser;
            ViewBag.nombreUser = nombreUser;
            ViewBag.idSucursalLogin = IdSucursal;
            ViewBag.idEmpresaLogin = idEmpresa;
            ViewBag.rolLogin = rol;

            //-------------------------------//
            // OPCIONES ELEGIDAS
            ViewBag.Elegir = true;
            ViewBag.EmpresaElegida = cboEmpresas;
            ViewBag.SucursalElegida = cboSucursales;

            if (rbReporte == "todos" || rbReporte == "pasteleria") {
                string capitalizedValue = char.ToUpper(rbReporte[0]) + rbReporte.Substring(1).ToLower();
                ViewBag.RadioElegida = "#rb" + capitalizedValue;
            } 
            else
            {
                string capitalizedValue = char.ToUpper(rbReporte[0]) + rbReporte.Substring(1).ToLower() + "s";
                ViewBag.RadioElegida = "#rb" + capitalizedValue;
            }
           


            ViewBag.FechaDesdeElegida = txtDesde;
            ViewBag.FechaHastaElegida = txtHasta;

            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Busqueda(string data)
        //{
        //    JObject modeloJson = JObject.Parse(data);
        //    int idUsuario = (int)modeloJson["idUsuario"];
        //    int idSucursal = (int)modeloJson["idSucursal"];
        //    int idEmpresa = (int)modeloJson["idEmpresa"];
        //    string tipo = (string)modeloJson["tipo"];
        //    string FechaDesde = (string)modeloJson["FechaDesde"];
        //    string FechaHasta = (string)modeloJson["FechaHasta"];
        //    //----------------------------------------------------//

        //    List<VMFabricaPedido> vmListaProductos = _mapper.Map<List<VMFabricaPedido>>(await _pedidosServices.ObtenerPedidos(
        //                                            Int32.Parse(idUsuario.ToString()), Int32.Parse(idSucursal.ToString()), Int32.Parse(idEmpresa.ToString()), 
        //                                            tipo, FechaDesde, FechaHasta));
            
        //    foreach(var i in vmListaProductos)
        //    {
        //        i.NombreSucursal = await _sucursalServices.ObtenerNombreSucursal(Int32.Parse(i.IdSucursal.ToString()));
        //        FabricaUsuario fu = await _usuariosServices.DatoUsuario(Int32.Parse(i.IdUsuario.ToString()));
        //        i.NombreUsuario = fu.Nombre;
        //    }

        //    return StatusCode(StatusCodes.Status200OK, new { data = vmListaProductos });
        //}

        [HttpGet]
        public async Task<IActionResult> CargarHelado(int idSucursal)
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
            List<VMFabricaPedidoDetalle> vmPedidoDetalle = _mapper.Map<List<VMFabricaPedidoDetalle>>(await _pedidosServices.ObtenerCodigoRealProducto(idPedido));
            return StatusCode(StatusCodes.Status200OK, vmPedidoDetalle);
        }

        //------------------------------------ ABM ---------------------------------------------------// 

        [HttpPost]
        public async Task<IActionResult> GuardarPedido([FromBody] List<VMPedido> modelo)
        {
            GenericResponse<VMFabricaPedido> gResponse = new GenericResponse<VMFabricaPedido>();

            try
            {
                ClaimsPrincipal claimPrin = HttpContext.User;
                string idUser = "";
                string IdSucursal = "";;

                if (claimPrin.Identity.IsAuthenticated) // se logeo??
                {

                    // OBTENGO ID USER AL LOGEAR POR MEDIO DEL CLAIMPS
                    idUser = claimPrin.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                    // OBTENGO IDSUCURSAL
                    IdSucursal = claimPrin.Claims.Where(c => c.Type == ClaimTypes.Surname).Select(c => c.Value).SingleOrDefault();
                }

                // SEPARAR
                VMFabricaPedido vm = new VMFabricaPedido();
                vm.Tipo = modelo[0].TipoCabecera;
                vm.Estado = modelo[0].EstadoCabecera;
                vm.Cantidad = modelo[0].CantidadCabecera;
                vm.Created = modelo[0].CreatedDetalle;
                vm.Remito = 0;
                vm.IdUsuario = Int32.Parse(idUser);
                vm.IdSucursal = Int32.Parse(IdSucursal);

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
                ClaimsPrincipal claimPrin = HttpContext.User;
                string idUser = "";
                string IdSucursal = ""; ;

                if (claimPrin.Identity.IsAuthenticated) // se logeo??
                {

                    // OBTENGO ID USER AL LOGEAR POR MEDIO DEL CLAIMPS
                    idUser = claimPrin.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                    // OBTENGO IDSUCURSAL
                    IdSucursal = claimPrin.Claims.Where(c => c.Type == ClaimTypes.Surname).Select(c => c.Value).SingleOrDefault();
                }

                // SEPARAR
                VMFabricaPedido vm = new VMFabricaPedido();
                vm.Tipo = modelo[0].TipoCabecera;
                vm.Estado = modelo[0].EstadoCabecera;
                vm.Cantidad = modelo[0].CantidadCabecera;
                vm.Created = modelo[0].CreatedDetalle;
                vm.Remito = 0;
                vm.IdUsuario =  Int32.Parse(idUser);
                vm.IdSucursal = Int32.Parse(IdSucursal);

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
            VMGeneralSucursales datoSucursal = _mapper.Map<VMGeneralSucursales>(await _empresasServices.ObtenerDatoSucursal(Int32.Parse(pedidoCabecera.IdSucursal.ToString())));

            // obtener codigo y cantidad
            List<VMFabricaPedidoDetalle> vmLista = _mapper.Map<List<VMFabricaPedidoDetalle>>(await _pedidosServices.VerDetallePedido(idPedido));

            // obtener nombre y categoria - al estar en otras tablas, obtenemos los resultado
            List<VMPedido> vmPedido = new List<VMPedido>();
            for(int a=0; a<vmLista.Count; a++)
            {
                VMPedido vm = new VMPedido();
                string resultado = await _pedidosServices.ObtenerNombreCategoriaProducto(Int32.Parse(vmLista[a].Codigo.ToString()), tipoPedido.Replace("\\", "").Replace("\"", "").ToLower(), Int32.Parse(datoSucursal.EmpresaId.ToString()));
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

        public IActionResult MostrarPDFPedido(int idPedido)
        {
            string urlPlantillaVista = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/PDFPedido?idPedido={idPedido}";
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings() { 
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = { 
                    new ObjectSettings(){ 
                        Page = urlPlantillaVista
                    }
                }
            };

            var archivoPDF = _converter.Convert(pdf);
            return File(archivoPDF, "application/pdf");
        }

        //------------------------------------  / ABM ---------------------------------------------------//

        private async Task<int> obtenerEmpresa(int idSucursal)
        {
            GeneralSucursales datoSucursal = await _sucursalServices.ObtenerDatosSucursal(idSucursal);
            return Int32.Parse(datoSucursal.EmpresaId.ToString());
        }
    }
}
