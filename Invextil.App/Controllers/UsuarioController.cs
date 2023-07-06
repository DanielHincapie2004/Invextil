using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft;
using Invextil.App.Models.ViewModels;
using Invextil.App.Utilidades.Response;
using Invextil.BLL.Interfaces;
using Invextil.Entity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Invextil.App.Controllers
{

    public class UsuarioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IEmpleadoService _empleadoService;
        private readonly IRolService _rolService;
        public UsuarioController(IMapper mapper, IEmpleadoService empleadoService, IRolService rolService)
        {
            _mapper = mapper;
            _empleadoService = empleadoService;
            _rolService = rolService;
        }
        public IActionResult Index()
        {
            return View();
        }
        //Peticiones http
        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            //Convertir al lisat de tipo rol a VMRol
            List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(await _rolService.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaRoles);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            //Convertir al lisat de tipo rol a VMRol
            List<VMEmpleado> vmEmpleadoLista = _mapper.Map<List<VMEmpleado>>(await _empleadoService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmEmpleadoLista });
        }
        [HttpGet]
        public async Task<IActionResult> ListaAsesor()
        {
            //Convertir al lisat de tipo rol a VMRol
            List<VMEmpleado> vmEmpleadoLista = _mapper.Map<List<VMEmpleado>>(await _empleadoService.ListaAsesor());
            return StatusCode(StatusCodes.Status200OK, new { data = vmEmpleadoLista });
        }



        [HttpPost]

        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMEmpleado> gResponse = new GenericResponse<VMEmpleado>();

            try
            {
                VMEmpleado vmEmpleado = JsonConvert.DeserializeObject<VMEmpleado>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    //Asiganr nombre a la foto
                    string nombre_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";
                Empleado empleadoCreado = await _empleadoService.Crear(_mapper.Map<Empleado>(vmEmpleado), fotoStream, nombreFoto, urlPlantillaCorreo);

                vmEmpleado = _mapper.Map<VMEmpleado>(empleadoCreado);

                gResponse.Estado = true;
                gResponse.Objeto = vmEmpleado;

            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]

        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMEmpleado> gResponse = new GenericResponse<VMEmpleado>();

            try
            {
                VMEmpleado vmEmpleado = JsonConvert.DeserializeObject<VMEmpleado>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    //Asiganr nombre a la foto
                    string nombre_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Empleado empleadoEncontrado = await _empleadoService.Editar(_mapper.Map<Empleado>(vmEmpleado), fotoStream, nombreFoto);

                vmEmpleado = _mapper.Map<VMEmpleado>(empleadoEncontrado);

                gResponse.Estado = true;
                gResponse.Objeto = vmEmpleado;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]

        public async Task<IActionResult> Eliminar(int idEmpleado)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _empleadoService.Eliminar(idEmpleado);
            }
            catch (Exception ex){
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
