using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Invextil.App.Models.ViewModels;
using Invextil.App.Utilidades.Response;
using Invextil.BLL.Interfaces;
using Invextil.Entity;
using Invextil.BLL.Implementacion;
using Microsoft.AspNetCore.Authorization;

namespace Invextil.App.Controllers
{
    [Authorize]
    public class CitasController : Controller
    {


        private readonly IMapper _mapper;
        private readonly ICitaService _citaService;

        public CitasController(IMapper mapper, ICitaService citaService)
        {
            _mapper = mapper;
            _citaService = citaService;
        }   
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMCita> vmCitaLista = _mapper.Map<List<VMCita>>(await _citaService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmCitaLista });
        }
        [HttpPost]

        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            GenericResponse<VMCita> gResponse = new GenericResponse<VMCita>();

            try
            {
                VMCita vmCita = JsonConvert.DeserializeObject<VMCita>(modelo);



                Cita citaCreada = await _citaService.Crear(_mapper.Map<Cita>(vmCita));

                vmCita = _mapper.Map<VMCita>(citaCreada);

                gResponse.Estado = true;
                gResponse.Objeto = vmCita;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]

        public async Task<IActionResult> Editar([FromForm] string modelo)
        {
            GenericResponse<VMCita> gResponse = new GenericResponse<VMCita>();

            try
            {
                VMCita vmCita = JsonConvert.DeserializeObject<VMCita>(modelo);

                Cita citaEncontrada = await _citaService.Editar(_mapper.Map<Cita>(vmCita));

                vmCita = _mapper.Map<VMCita>(citaEncontrada);

                gResponse.Estado = true;
                gResponse.Objeto = vmCita;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]

        public async Task<IActionResult> Eliminar(int idCita)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _citaService.Eliminar(idCita);
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
