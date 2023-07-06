using AutoMapper;
using Invextil.App.Models.ViewModels;
using Invextil.App.Utilidades.Response;
using Invextil.BLL.Implementacion;
using Invextil.BLL.Interfaces;
using Invextil.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Invextil.App.Controllers
{
    [Authorize]
    public class MuestrasController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IMuestraService _muestraService;

        public MuestrasController(IMapper mapper, IMuestraService muestraService)
        {
            _mapper = mapper;
            _muestraService = muestraService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMMuestra> vmMuestraLista = _mapper.Map<List<VMMuestra>>(await _muestraService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmMuestraLista });
        }

        [HttpPost]

        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            GenericResponse<VMMuestra> gResponse = new GenericResponse<VMMuestra>();

            try
            {
                VMMuestra vmMuestra = JsonConvert.DeserializeObject<VMMuestra>(modelo);



                Muestra muestraCreada = await _muestraService.Crear(_mapper.Map<Muestra>(vmMuestra));

                vmMuestra = _mapper.Map<VMMuestra>(muestraCreada);

                gResponse.Estado = true;
                gResponse.Objeto = vmMuestra;

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
            GenericResponse<VMMuestra> gResponse = new GenericResponse<VMMuestra>();

            try
            {
                VMMuestra vmMuestra = JsonConvert.DeserializeObject<VMMuestra>(modelo);

                Muestra muestraCreada = await _muestraService.Editar(_mapper.Map<Muestra>(vmMuestra));

                vmMuestra = _mapper.Map<VMMuestra>(muestraCreada);

                gResponse.Estado = true;
                gResponse.Objeto = vmMuestra;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]

        public async Task<IActionResult> Eliminar(int idMuestra)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _muestraService.Eliminar(idMuestra);
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
