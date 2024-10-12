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
        private readonly UsuarioService usuario;
        public HomeController(ILogger<HomeController> logger, ClienteService cliente, UsuarioService usuario)
        {
            _logger = logger;
            this.cliente = cliente;
            this.usuario = usuario;
        }

        public async Task<IActionResult> Index(int? idproyecto, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto)
        {
            var id = HttpContext.Session.GetInt32("id");
            if (id.HasValue)
             {
                 var clientes = await cliente.ObtenerClientesIndex(0);
                var usuariosproyectos = await usuario.ObtenerUsuariosProyecto(id.Value);   

                 ViewBag.clientes = clientes;
                ViewBag.Proyectos = usuariosproyectos;
                 return View();
             }
             else
             {
                 return RedirectToAction("Login","Usuario");
             }
            /*var clientes = await cliente.ObtenerClientesIndex(0);
            ViewBag.clientes = clientes;
            return View();*/
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
