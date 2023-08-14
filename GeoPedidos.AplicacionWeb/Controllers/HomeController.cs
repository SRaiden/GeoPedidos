using GeoPedidos.AplicacionWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

//using AutoMapper;
//using GeoPedidos.AplicacionWeb.Models.ViewModels;
using GeoPedidos.AplicacionWeb.Utilidades.Response;
using GeoPedidos.BLL.Interfaces;
using GeoPedidos.Entity;
using GeoPedidos.AplicacionWeb.Models.ViewModels;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;

namespace GeoPedidos.AplicacionWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IFabricaUsuariosServices _usuariosServices;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IMapper mapper, IFabricaUsuariosServices usuariosServices)
        {
            _logger = logger;
            _mapper = mapper;
            _usuariosServices = usuariosServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // termino sesion
            return RedirectToAction("Login", "Acceso");
        }

        public IActionResult Perfil()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerUser()
        {
            GenericResponse<VMFabricaUsuario> response = new GenericResponse<VMFabricaUsuario>();
            try
            {
                ClaimsPrincipal claimPrin = HttpContext.User;
                string idUser = claimPrin.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                VMFabricaUsuario usuario = _mapper.Map<VMFabricaUsuario>(await _usuariosServices.ObtenerId(int.Parse(idUser)));
                response.Estado = true;
                response.Objeto = usuario;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromBody] VMFabricaUsuario modelo)
        {
            GenericResponse<VMFabricaUsuario> response = new GenericResponse<VMFabricaUsuario>();
            try
            {
                ClaimsPrincipal claimPrin = HttpContext.User;
                string idUser = claimPrin.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                FabricaUsuario entidad = _mapper.Map<FabricaUsuario>(modelo);
                entidad.Id = int.Parse(idUser);

                bool resultado = false;
                if (modelo.Contraseña.IsNullOrEmpty()) resultado = await _usuariosServices.GuardarPerfil(entidad);
                else resultado = await _usuariosServices.CambiarClave(int.Parse(idUser), modelo.Contraseña, modelo.ContraseñaNueva);

                response.Estado = resultado;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}