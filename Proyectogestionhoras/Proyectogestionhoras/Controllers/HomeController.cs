using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Models;
using System.Diagnostics;
using Proyectogestionhoras.Services;
namespace Proyectogestionhoras.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClienteService cliente;
        public HomeController(ILogger<HomeController> logger, ClienteService cliente)
        {
            _logger = logger;
            this.cliente = cliente;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id.HasValue)
            {
                var clientes = await cliente.ObtenerClientesIndex(0);
                ViewBag.clientes = clientes;
                return View();
            }
            else
            {
                return RedirectToAction("Login","Usuario");
            }
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
