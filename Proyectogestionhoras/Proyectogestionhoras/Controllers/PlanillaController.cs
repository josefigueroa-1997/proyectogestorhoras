using Microsoft.AspNetCore.Mvc;

namespace Proyectogestionhoras.Controllers
{
    public class PlanillaController : Controller
    {
        public IActionResult PlanillaRegistro()
        {
            var id = HttpContext.Session.GetInt32("id");
            if(id.HasValue)
            {
                return View("Planilla");

            }
            else
            {
                return View("Login","Usuario");
            }
            
        }
    }
}
