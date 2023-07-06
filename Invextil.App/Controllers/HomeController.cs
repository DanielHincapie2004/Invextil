using Invextil.App.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using System.Security.Claims;

using AutoMapper;
using Invextil.App.Models.ViewModels;
using Invextil.App.Utilidades.Response;
using Invextil.BLL.Interfaces;
using Invextil.Entity;

namespace Invextil.App.Controllers
{

    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _loger;

        private readonly IEmpleadoService _empleadoService;
        private readonly IMapper _mapper;
        public HomeController(IEmpleadoService empleadoService, IMapper mapper, ILogger<HomeController> loger)
        {
            _empleadoService = empleadoService;
            _mapper = mapper;
            _loger = loger;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Perfil()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<VMEmpleado> response = new GenericResponse<VMEmpleado>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                VMEmpleado usuario = _mapper.Map<VMEmpleado>(await _empleadoService.ObtenerPorId(int.Parse(idUsuario)));

                response.Estado = true;
                response.Objeto = usuario;

            }
            catch(Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromBody] VMEmpleado modelo)
        {
            GenericResponse<VMEmpleado> response = new GenericResponse<VMEmpleado>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                Empleado entidad = _mapper.Map<Empleado>(modelo);
                entidad.IdEmpleado = int.Parse(idUsuario);

                bool resultado = await _empleadoService.GuardarPerfil(entidad);

                response.Estado = resultado;

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarClave([FromBody] VMCambiarClave modelo)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();


                bool resultado = await _empleadoService.CambiarClave(
                    int.Parse(idUsuario), 
                    modelo.claveActual, 
                    modelo.claveNueva
                    );

                response.Estado = resultado;

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso");
        }
    }
}