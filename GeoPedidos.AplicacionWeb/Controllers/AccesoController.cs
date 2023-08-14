using Microsoft.AspNetCore.Mvc;

using GeoPedidos.AplicacionWeb.Models.ViewModels;
using GeoPedidos.BLL.Interfaces;
using GeoPedidos.Entity;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace GeoPedidos.AplicacionWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IFabricaUsuariosServices _fabricaUsuarioServices;

        public AccesoController(IFabricaUsuariosServices fabricaUsuarioServices)
        {
            _fabricaUsuarioServices = fabricaUsuarioServices;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated) // BOTON DE MANTENER SESION ABIERTA
            {
                return RedirectToAction("Index", "Pedidos");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMFabricaUsuarioLogin modelo)
        {
            FabricaUsuario usuarioEncontrado = await _fabricaUsuarioServices.ObtenerCredenciales(modelo.Correo, modelo.Clave);
            if (usuarioEncontrado == null)
            {
                ViewData["Mensaje"] = "No existe este usuario";
                return View();
            }

            ViewData["Mensaje"] = null;

            int rol = 0;
            if (usuarioEncontrado.Rol == "user") rol = 1;

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuarioEncontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuarioEncontrado.Id.ToString()),
                new Claim(ClaimTypes.Role, rol.ToString()),
                new Claim(ClaimTypes.Surname, usuarioEncontrado.IdSucursal.ToString())
            };
            ClaimsIdentity cIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = (Boolean)modelo.MantenerSesion
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(cIdentity),
                properties);

            return RedirectToAction("Index", "Pedidos");
        }

        [HttpGet]
        public IActionResult RestablecerClave()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerClave(VMFabricaUsuarioLogin modelo)
        {
            try
            {
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";
                bool resultado = await _fabricaUsuarioServices.RestablecerClave(modelo.Correo, urlPlantillaCorreo);

                if (resultado)
                {
                    ViewData["Mensaje"] = "Contraseña restablecida. Revise su correo";
                    ViewData["MensajeError"] = null;
                }
                else
                {
                    ViewData["Mensaje"] = null;
                    ViewData["MensajeError"] = "No se pudo restablecer la contraseña. Por favor intente de nuevo mas tarde.";
                }
            }
            catch (Exception ex)
            {
                ViewData["Mensaje"] = null;
                ViewData["MensajeError"] = ex.Message;
            }

            return View();
        }
    }
}
