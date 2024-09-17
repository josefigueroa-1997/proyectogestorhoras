using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Services;
using System.Diagnostics;
namespace Proyectogestionhoras.Controllers
{
    public class ContactoController : Controller
    {
        private readonly ClienteService clienteService;
        private readonly ContactoService contactoService;
        public ContactoController(ClienteService clienteService,ContactoService contactoService)
        {
            this.clienteService = clienteService;
            this.contactoService = contactoService;
        }
        [HttpGet]
        public async Task<IActionResult> ContactosCliente(int idcliente)
        {
            var contactosclientes = await contactoService.ObtenerContactos(null,idcliente,null);
            ViewBag.Contactos = contactosclientes;
            return View("ContactosCliente");
        }

        public async Task <IActionResult> AgregarContacto(int idcliente)
        {
            
            var cliente = await clienteService.ObtenerClientesIndex(idcliente);
           ViewBag.Cliente = cliente;
         
            return View();

          
        }
        [HttpPost]
        public async Task<IActionResult> AgregarContacto(string nombre, string cargo, DateOnly desde, DateOnly hasta, string email, string telefono, int idc)
        {
            try
            {
                bool resultado = await contactoService.RegistrarContacto(nombre,cargo, desde, hasta, email, telefono, idc);
                if (resultado) {
                    return RedirectToAction("ProyectosCliente", "Cliente", new {idcliente=idc});
                
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex) {

                Debug.WriteLine($"Hubo un error al registrar el contacot:"+ex.Message);
                return View();
            }
        }

        public async Task<IActionResult> DetalleContacto(int id)
        {
            var contacto = await contactoService.ObtenerContactos(id, 0, null);
            ViewBag.Contactos = contacto;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DetalleContacto(int id, string nombre, string cargo, DateOnly desde, DateOnly hasta, string email, string telefono, int idc)
        {
            try
            {
                bool resultado = await contactoService.EditarContacto(id,nombre,cargo,desde,hasta,email,telefono,idc);
                if (resultado) {
                    Debug.WriteLine(idc);
                    return RedirectToAction("ProyectosCliente", "Cliente", new { idcliente = idc });
                   
                }
                else
                {
                    return View();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message); return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarContacto(int id)
        {
            try
            {
                bool resultado = await contactoService.EliminarContacto(id);
                if (resultado) {
                    
                    return RedirectToAction("ProyectosCliente", "Cliente", new {idcliente=HttpContext.Session.GetInt32("IdCliente")});
                
                }
                else
                {
                    Debug.WriteLine("Hubo un rrrr");
                    return View();
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Hubo un error al eliminar al contacto"+e.Message);
                return View();
            }
        }
    }
}
