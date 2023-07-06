using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using Invextil.App.Models.ViewModels;
using Invextil.App.Utilidades.Response;
using Invextil.BLL.Interfaces;
using Invextil.Entity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Invextil.App.Controllers
{
    [Authorize]
    public class FamiliaController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IFamiliaService _familiaService;

        public FamiliaController(IMapper mapper, IFamiliaService familiaService)
        {
            _mapper = mapper;
            _familiaService = familiaService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMFamilia> vmFamiliaLista = _mapper.Map<List<VMFamilia>> (await _familiaService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmFamiliaLista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody]VMFamilia modelo)
        {
            GenericResponse<VMFamilia> gResponse = new GenericResponse<VMFamilia> ();

            try
            {
                Familia familiaCreada = await _familiaService.Crear(_mapper.Map<Familia>(modelo));
                modelo = _mapper.Map<VMFamilia>(familiaCreada);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMFamilia modelo)
        {
            GenericResponse<VMFamilia> gResponse = new GenericResponse<VMFamilia>();

            try
            {
                Familia familiaEditada = await _familiaService.Editar(_mapper.Map<Familia>(modelo));
                modelo = _mapper.Map<VMFamilia>(familiaEditada);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar( int idFamilia)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _familiaService.Eliminar(idFamilia);
            }catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
