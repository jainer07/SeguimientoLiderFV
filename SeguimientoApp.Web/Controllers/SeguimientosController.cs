using Microsoft.AspNetCore.Mvc;

namespace SeguimientoApp.Web.Controllers
{
    public class SeguimientosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
