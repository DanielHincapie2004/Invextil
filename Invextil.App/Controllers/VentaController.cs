using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Invextil.App.Models.ViewModels;
using Invextil.App.Utilidades.Response;
using Invextil.BLL.Interfaces;
using Invextil.Entity;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Invextil.App.Controllers
{
    [Authorize]
    public class VentaController : Controller
    {
        private readonly ITipoDocumentoService _tipoDocumento;
        private readonly IMapper _mapper;
        private readonly ICotizacionService _cotizacion;
        private readonly IConverter _converter;

        public VentaController(ITipoDocumentoService tipoDocumento, IMapper mapper, ICotizacionService cotizacion, IConverter converter)
        {
            _tipoDocumento = tipoDocumento;
            _mapper = mapper;
            _cotizacion = cotizacion;
            _converter = converter;
        }

        public IActionResult NuevaVenta()
        {
            return View();
        }

        public IActionResult HistorialVenta()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoDocumento()
        {
            List<VMTipoDocumentoVenta> vmTipoDocumento = _mapper.Map<List<VMTipoDocumentoVenta>>(await _tipoDocumento.Lista());
            return StatusCode(StatusCodes.Status200OK, vmTipoDocumento);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTela(string busqueda)
        {
            List<VMTela> vmListaTela = _mapper.Map<List<VMTela>>(await _cotizacion.ObtenerTelas(busqueda));
            return StatusCode(StatusCodes.Status200OK, vmListaTela);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarCotizacion([FromBody] VMVenta modelo)
        {
            GenericResponse<VMVenta> gResponse = new GenericResponse<VMVenta>();
            
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();
                modelo.IdEmpleado = int.Parse(idUsuario);
                Venta ventaCreada = await _cotizacion.Registrar(_mapper.Map<Venta>(modelo));
                modelo = _mapper.Map<VMVenta>(ventaCreada);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            List<VMVenta> vmHistorialVenta = _mapper.Map<List<VMVenta>>(await _cotizacion.Historial(numeroVenta, fechaInicio,fechaFin));
            return StatusCode(StatusCodes.Status200OK, vmHistorialVenta);
        }

        public IActionResult MostrarPDFVenta(string numeroVenta)
        {
            string urlPlantilla = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/PDFCotizacion?numeroVenta={numeroVenta}";
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        Page = urlPlantilla
                    }
                }
            };

            var archivoPdf = _converter.Convert(pdf);
            return File(archivoPdf,"application/pdf");
        }
    }
}
