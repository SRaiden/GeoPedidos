using GeoPedidos.BLL.Interfaces;
using GeoPedidos.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace GeoPedidos.AplicacionWeb.Utilidades.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IFabricaUsuariosServices _fabricaUsuariosServices;

        public MenuViewComponent(IFabricaUsuariosServices fabricaUsuariosServices)
        {
            _fabricaUsuariosServices = fabricaUsuariosServices;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimPrin = HttpContext.User;

            string idUser = "";
            string rol = "";

            if (claimPrin.Identity.IsAuthenticated) { // se logeo??
                // OBTENGO ID USER AL LOGEAR POR MEDIO DEL CLAIMPS
                idUser = claimPrin.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();

                // OBTENGO ROL (0 -> ADMIN, 1 -> USER)
                rol = claimPrin.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();
            }

            ViewBag.rol = rol;
            ViewBag.user = idUser;

            return View();
        }
    }
}
