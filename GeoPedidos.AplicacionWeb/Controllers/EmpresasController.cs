using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using GeoPedidos.AplicacionWeb.Models.ViewModels;
using GeoPedidos.AplicacionWeb.Utilidades.Response;
using GeoPedidos.BLL.Interfaces;
using GeoPedidos.Entity;
using Microsoft.AspNetCore.Authorization;

namespace GeoPedidos.AplicacionWeb.Controllers
{
    [Authorize]
    public class EmpresasController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IEmpresasServices _empresasServices;

        public EmpresasController(IMapper mapper, IEmpresasServices empresasServices)
        {
            _mapper = mapper;
            _empresasServices = empresasServices;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMGeneralEmpresa> vmEmpresaLista = _mapper.Map<List<VMGeneralEmpresa>>(await _empresasServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmEmpresaLista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VMGeneralEmpresa modelo)
        {
            GenericResponse<VMGeneralEmpresa> gResponse = new GenericResponse<VMGeneralEmpresa>();

            try
            {
                // Obtener Ultimo Id
                int id = await _empresasServices.UltimoId();
                modelo.Id = id + 1;

                GeneralEmpresa empresaCreada = await _empresasServices.Crear(_mapper.Map<GeneralEmpresa>(modelo));
                modelo = _mapper.Map<VMGeneralEmpresa>(empresaCreada);

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
        public async Task<IActionResult> Editar([FromBody] VMGeneralEmpresa modelo)
        {
            GenericResponse<VMGeneralEmpresa> gResponse = new GenericResponse<VMGeneralEmpresa>();

            try
            {
                GeneralEmpresa empresaEditada = await _empresasServices.Editar(_mapper.Map<GeneralEmpresa>(modelo));
                modelo = _mapper.Map<VMGeneralEmpresa>(empresaEditada);

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
                gResponse.Estado = await _empresasServices.BajaLogica(id);
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
