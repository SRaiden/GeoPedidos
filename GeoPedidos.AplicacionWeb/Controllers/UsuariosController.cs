using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using GeoPedidos.AplicacionWeb.Models.ViewModels;
using GeoPedidos.AplicacionWeb.Utilidades.Response;
using GeoPedidos.BLL.Interfaces;
using GeoPedidos.Entity;
using Microsoft.AspNetCore.Authorization;
using GeoPedidos.BLL.Implementacion;
using System.Security.Claims;

namespace GeoPedidos.AplicacionWeb.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IFabricaUsuariosServices _fabricaUsuarioServices;
        private readonly IEmpresasServices _empresasServices;
        private readonly ISucursalServices _sucursalesServices;

        public UsuariosController(IMapper mapper, IFabricaUsuariosServices fabricaUsuarioServices, IEmpresasServices empresasServices, ISucursalServices sucursalesServices)
        {
            _mapper = mapper;
            _fabricaUsuarioServices = fabricaUsuarioServices;
            _empresasServices = empresasServices;
            _sucursalesServices = sucursalesServices;
        }

        public async Task<IActionResult> Index()
        {
            // PARA OPCIONES DE QUE MOSTRAR EN EL LOGIN
            ClaimsPrincipal claimPrin = HttpContext.User;
            string idEmpresa = "";
            string idUser = "";

            if (claimPrin.Identity.IsAuthenticated) // se logeo??
            {
                // OBTENGO ID USER AL LOGEAR POR MEDIO DEL CLAIMPS
                idUser = claimPrin.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                // Busco el IDEMPRESA (ESTO ES PARA SUPERADMIN)
                FabricaUsuario datoUsuario = await _fabricaUsuarioServices.DatoUsuario(Int32.Parse(idUser));
                idEmpresa = datoUsuario.IdEmpresa.ToString();
            }

            ViewBag.idUserLogin = idUser;
            ViewBag.idEmpresaLogin = idEmpresa;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista(int idEmpresa = 0)
        {
            List<VMFabricaUsuario> vmUsuarioLista = null;
            if (idEmpresa == 0) {
                vmUsuarioLista = _mapper.Map<List<VMFabricaUsuario>>(await _fabricaUsuarioServices.Lista());
            }
            else
            {
                vmUsuarioLista = _mapper.Map<List<VMFabricaUsuario>>(await _fabricaUsuarioServices.Lista(idEmpresa));
            }
            foreach (var a in vmUsuarioLista)
            {
                a.NombreEmpresa = a.IdEmpresa == 0? "Administrador de Empresa" : await _empresasServices.ObtenerNombreEmpresa(Int32.Parse(a.IdEmpresa.ToString()));
                a.NombreSucursal = a.IdSucursal == 0 ? "Administrador de Sucursales" : await _sucursalesServices.ObtenerNombreSucursal(Int32.Parse(a.IdSucursal.ToString()));
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmUsuarioLista });
        }

        public async Task<IActionResult> DatoEmpresa(int idUsuario = 0) 
        {

            VMFabricaUsuario vmUsuarioLista = _mapper.Map<VMFabricaUsuario>(await _fabricaUsuarioServices.DatoUsuario(idUsuario));
            VMGeneralEmpresa vmDatoEmpresa = _mapper.Map<VMGeneralEmpresa>(await _empresasServices.ObtenerDatoEmpresa(Int32.Parse(vmUsuarioLista.IdEmpresa.ToString())));


            return StatusCode(StatusCodes.Status200OK, vmDatoEmpresa);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodasEmpresas()
        {
            List<VMGeneralEmpresa> vmListaTipoDocumento = _mapper.Map<List<VMGeneralEmpresa>>(await _empresasServices.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaTipoDocumento);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerSucursalEmpresa(int idEmpresa)
        {
            List<VMGeneralSucursales> vmListaTipoDocumento = _mapper.Map<List<VMGeneralSucursales>>(await _sucursalesServices.Lista(idEmpresa));
            return StatusCode(StatusCodes.Status200OK, vmListaTipoDocumento);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VMFabricaUsuario modelo)
        {
            GenericResponse<VMFabricaUsuario> gResponse = new GenericResponse<VMFabricaUsuario>();

            try
            {
                // Obtener Ultimo Id
                int id = await _fabricaUsuarioServices.UltimoId();
                modelo.Id = id + 1;

                FabricaUsuario usuarioCreado = await _fabricaUsuarioServices.Crear(_mapper.Map<FabricaUsuario>(modelo));
                modelo = _mapper.Map<VMFabricaUsuario>(usuarioCreado);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Editar([FromBody] VMFabricaUsuario modelo)
        {
            GenericResponse<VMFabricaUsuario> gResponse = new GenericResponse<VMFabricaUsuario>();

            try
            {
                // no puedo editar sin antes al campo created mandarle un parametro de formato datetime, asi
                // que busco el campo de base de datos y se lo coloco nuevamente
                FabricaUsuario usuario = await _fabricaUsuarioServices.DatoUsuario(modelo.Id);
                modelo.Created = usuario.Created.ToString();
                if (Boolean.Parse(usuario.OkLogin.ToString())) modelo.OkLogin = 1;
                else modelo.OkLogin = 0;

                FabricaUsuario usuarioEditado = await _fabricaUsuarioServices.Editar(_mapper.Map<FabricaUsuario>(modelo));
                modelo = _mapper.Map<VMFabricaUsuario>(usuarioEditado);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _fabricaUsuarioServices.BajaLogica(id);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
