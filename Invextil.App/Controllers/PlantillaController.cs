using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Invextil.App.Models.ViewModels;
using Invextil.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Invextil.App.Controllers
{
    public class PlantillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICotizacionService _cotizacionRepository;
        public PlantillaController(IMapper mapper, ICotizacionService cotizacionRepository)
        {
            _mapper=mapper;
            _cotizacionRepository = cotizacionRepository;
        }
        public IActionResult EnviarClave(string correo, string clave)
        {
            //View data permite compartir info con la vista
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";
            return View();
        }

        public async Task<IActionResult> PDFCotizacion(string numeroVenta)
        {
            VMVenta vmVenta = _mapper.Map<VMVenta>(await _cotizacionRepository.Detalle(numeroVenta));
            VMPDFVenta modelo = new VMPDFVenta();
            modelo.venta = vmVenta;
            return View(modelo);
        }

        public IActionResult RestablecerClave(string clave)
        {
            ViewData["Clave"] = clave;
            return View();
        }
    }
}
