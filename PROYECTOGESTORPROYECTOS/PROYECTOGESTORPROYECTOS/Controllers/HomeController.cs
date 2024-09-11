using Microsoft.AspNetCore.Mvc;
using PROYECTOGESTORPROYECTOS.Models;
namespace PROYECTOGESTORPROYECTOS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
