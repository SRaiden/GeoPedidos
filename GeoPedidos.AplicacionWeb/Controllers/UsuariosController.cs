using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using GeoPedidos.AplicacionWeb.Models.ViewModels;
using GeoPedidos.AplicacionWeb.Utilidades.Response;
using GeoPedidos.BLL.Interfaces;
using GeoPedidos.Entity;
using Microsoft.AspNetCore.Authorization;
using GeoPedidos.BLL.Implementacion;

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

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMFabricaUsuario> vmUsuarioLista = _mapper.Map<List<VMFabricaUsuario>>(await _fabricaUsuarioServices.Lista());
            foreach (var a in vmUsuarioLista)
            {
                a.NombreEmpresa = a.IdEmpresa == 0? "Administrador de Empresa" : await _empresasServices.ObtenerNombreEmpresa(Int32.Parse(a.IdEmpresa.ToString()));
                a.NombreSucursal = a.IdSucursal == 0 ? "Administrador de Sucursales" : await _sucursalesServices.ObtenerNombreSucursal(Int32.Parse(a.IdSucursal.ToString()));
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmUsuarioLista });
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
                modelo.OkLogin = Int32.Parse(usuario.OkLogin.ToString());

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
