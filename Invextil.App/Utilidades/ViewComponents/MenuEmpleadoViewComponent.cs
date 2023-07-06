using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Invextil.App.Utilidades.ViewComponents
{
    public class MenuEmpleadoViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            string nombreEmpleado = "";
            string urlFoto = "";

            if (claimUser.Identity.IsAuthenticated)
            {
                nombreEmpleado = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.Name)
                    .Select(c => c.Value).SingleOrDefault();

                urlFoto = ((ClaimsIdentity)claimUser.Identity).FindFirst("UrlFoto").Value;
            }

            ViewData["nombreUsuario"] = nombreEmpleado;
            ViewData["urlFoto"] = urlFoto;

            return View();
        }
    }
}
