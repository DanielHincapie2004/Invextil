using Microsoft.AspNetCore.Mvc;
using Invextil.App.Models.ViewModels;
using Invextil.BLL.Interfaces;
using Invextil.Entity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace Invextil.App.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IEmpleadoService _empleadoService;
        public AccesoController(IEmpleadoService empleadoService)
        {
            _empleadoService = empleadoService;
        }
        public IActionResult Login()
        {

            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "DashBoard");
            }
            return View();
        }

        public IActionResult RestablecerClave()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(VMEmpleadoLogin modelo)
            {
            Empleado empleadoEncontra = await _empleadoService.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);
            if(empleadoEncontra == null)
            {
                ViewData["Mensaje"] = "El empleado no existe";
                return View();
            }

            ViewData["Mensaje"] = null;
            List<Claim> claims = new List<Claim>() { 
                new Claim(ClaimTypes.Name, empleadoEncontra.Nombre),
                new Claim(ClaimTypes.NameIdentifier, empleadoEncontra.IdEmpleado.ToString()),
                new Claim(ClaimTypes.Role, empleadoEncontra.IdRol.ToString()),
                new Claim("UrlFoto", empleadoEncontra.UrlFoto)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = modelo.MantenerSesion
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity),
                properties
                );

            return RedirectToAction("Index", "DashBoard");
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerClave(VMEmpleadoLogin modelo)
        {
            try
            {
                string urlPlantillaCoreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";
                bool resultado = await _empleadoService.RestablecerClave(modelo.Correo, urlPlantillaCoreo);

                if (resultado)
                {
                    ViewData["Mensaje"] = "Su contraseña fue restablecida. Revise su correo electronico";
                    ViewData["MensajeError"] = null;
                }
                else
                {
                    ViewData["Mensaje"] = null;
                    ViewData["MensajeError"] = "Error. Intentelo de neuvo mas tarde";
                }
            }
            catch(Exception ex)
            {
                ViewData["Mensaje"] = null;
                ViewData["MensajeError"] = ex.Message;
            }
            return View();
        }
    }
}
