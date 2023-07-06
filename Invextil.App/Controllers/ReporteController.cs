using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invextil.App.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
