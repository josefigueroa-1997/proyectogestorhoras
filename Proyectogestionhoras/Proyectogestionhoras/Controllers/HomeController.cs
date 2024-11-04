using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Models;
using System.Diagnostics;
using Proyectogestionhoras.Services;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
namespace Proyectogestionhoras.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClienteService cliente;
        private readonly UsuarioService usuario;
        private readonly PlanillaService planilla;
        public HomeController(ILogger<HomeController> logger, ClienteService cliente, UsuarioService usuario, PlanillaService planilla)
        {
            _logger = logger;
            this.cliente = cliente;
            this.usuario = usuario;
            this.planilla = planilla;
        }

        public async Task<IActionResult> Index()
        {
             var id = HttpContext.Session.GetInt32("id");
             if (id.HasValue)
             {
                 var rol = HttpContext.Session.GetInt32("idrol");
                 if (rol.HasValue)
                 {
                     if (rol == 1)
                     {
                         var clientes = await cliente.ObtenerClientesIndex(0);
                         var usuariosproyectos = await usuario.ObtenerUsuariosProyecto(id.Value);

                         ViewBag.clientes = clientes;
                         ViewBag.Proyectos = usuariosproyectos;
                         return View();
                     }
                     else
                     {
                         var planillas = await planilla.ObtenerPlanillaUsuario(id.Value, 0);
                         ViewBag.Planilla = planillas;
                         return RedirectToAction("PlanillaRegistro","Planilla");
                     }
                 }
             }


             return RedirectToAction("Login", "Usuario");
            

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
