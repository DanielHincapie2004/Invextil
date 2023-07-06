using Microsoft.AspNetCore.Mvc;

using Invextil.App.Models.ViewModels;
using Invextil.App.Utilidades.Response;
using Invextil.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Invextil.App.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly IDashBoardService _dashboardService;


        public DashBoardController(IDashBoardService dashboardService)
        {
            _dashboardService = dashboardService;   
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task <IActionResult> ObtenerResumen()
        {
            GenericResponse<VMDashboard> genericResponse = new GenericResponse<VMDashboard>();

            try
            {
                VMDashboard vMDashboard = new VMDashboard();

                vMDashboard.TotalVentas = await _dashboardService.TotalCotizacionesUltimaSemana();
                vMDashboard.TotalProductos = await _dashboardService.TotalTelas();
                vMDashboard.TotalCategorias= await _dashboardService.TotalFamilias();

                List<VMVentasSemana> listaVentasSemana = new List<VMVentasSemana>();
                List<VMProductosSemana> listaProductosSemana = new List<VMProductosSemana>();

                foreach (KeyValuePair<string, int> item in await _dashboardService.CotizacionesUltimaSemana())
                {
                    listaVentasSemana.Add(new VMVentasSemana()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                }
                
                foreach (KeyValuePair<string, int> item in await _dashboardService.TelasTopUltimaSemana())
                {
                    listaProductosSemana.Add(new VMProductosSemana()
                    {
                        Producto = item.Key,
                        Cantidad = item.Value
                    });
                }

                vMDashboard.VentasUltimaSemana = listaVentasSemana;
                vMDashboard.ProductosTopUltimaSemana = listaProductosSemana;

                genericResponse.Estado = true;
                genericResponse.Objeto = vMDashboard;
            }
            catch (Exception ex) 
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

    }
}
