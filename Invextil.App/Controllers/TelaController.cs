using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Invextil.App.Models.ViewModels;
using Invextil.App.Utilidades.Response;
using Invextil.BLL.Interfaces;
using Invextil.Entity;
using Microsoft.AspNetCore.Authorization;

namespace Invextil.App.Controllers
{
    [Authorize]
    public class TelaController : Controller
    {

        private readonly IMapper _mapper;
        private readonly ITelaService _telaService;

        public TelaController(IMapper mapper, ITelaService telaService)
        {
            _mapper = mapper;
            _telaService = telaService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMTela> vmTelaLista = _mapper.Map<List<VMTela>>(await _telaService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmTelaLista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile imagen, [FromForm] string modelo)
        {

            GenericResponse<VMTela> gResponse = new GenericResponse<VMTela>();

            try 
            {
                VMTela vmTela = JsonConvert.DeserializeObject<VMTela>(modelo);
                string nombreImagen = "";
                Stream imagenStream = null;

                if(imagen != null)
                {
                    string nombreCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreImagen = string.Concat(nombreCodigo, extension);
                    imagenStream = imagen.OpenReadStream();
                }

                Tela telaCreada = await _telaService.Crear(_mapper.Map<Tela>(vmTela),imagenStream, nombreImagen);

                vmTela = _mapper.Map<VMTela>(telaCreada);
                gResponse.Estado = true;
                gResponse.Objeto = vmTela;
            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }


        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile imagen, [FromForm] string modelo)
        {

            GenericResponse<VMTela> gResponse = new GenericResponse<VMTela>();

            try
            {
                VMTela vmTela = JsonConvert.DeserializeObject<VMTela>(modelo);

                string nombreFoto = "";
                Stream imagenStream = null;

                if (imagen != null)
                {
                    string nombre_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreFoto = string.Concat(nombre_codigo, extension);
                    imagenStream = imagen.OpenReadStream();
                }

                Tela telaEditada = await _telaService.Editar(_mapper.Map<Tela>(vmTela),imagenStream);

                vmTela = _mapper.Map<VMTela>(telaEditada);
                gResponse.Estado = true;
                gResponse.Objeto = vmTela;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]

        public async Task<IActionResult> Eliminar(int idTela)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _telaService.Eliminar(idTela);
            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }
    }
}
